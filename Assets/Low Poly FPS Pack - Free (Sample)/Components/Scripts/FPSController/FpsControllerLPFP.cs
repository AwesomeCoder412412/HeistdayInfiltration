using System;
using System.Linq;
using UnityEngine;
using Mirror;
using SimpleFirebaseUnity;

namespace FPSControllerLPFP
{
    /// Manages a first person character
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(AudioSource))]
    public class FpsControllerLPFP : NetworkBehaviour
    {
#pragma warning disable 649
		[Header("Arms")]
        [Tooltip("The transform component that holds the gun camera."), SerializeField]
        private Transform arms;

        [Tooltip("The position of the arms and gun camera relative to the fps controller GameObject."), SerializeField]
        private Vector3 armPosition;

		[Header("Audio Clips")]
        [Tooltip("The audio clip that is played while walking."), SerializeField]
        private AudioClip walkingSound;

        [Tooltip("The audio clip that is played while running."), SerializeField]
        private AudioClip runningSound;

		[Header("Movement Settings")]
        [Tooltip("How fast the player moves while walking and strafing."), SerializeField]
        private float walkingSpeed = 5f;

        [Tooltip("How fast the player moves while running."), SerializeField]
        private float runningSpeed = 9f;

        [Tooltip("Approximately the amount of time it will take for the player to reach maximum running or walking speed."), SerializeField]
        private float movementSmoothness = 0.125f;

        [Tooltip("Amount of force applied to the player when jumping."), SerializeField]
        private float jumpForce = 35f;

		[Header("Look Settings")]
        [Tooltip("Rotation speed of the fps controller."), SerializeField]
        private float mouseSensitivity = 7f;

        [Tooltip("Approximately the amount of time it will take for the fps controller to reach maximum rotation speed."), SerializeField]
        private float rotationSmoothness = 0.05f;

        [Tooltip("Minimum rotation of the arms and camera on the x axis."),
         SerializeField]
        private float minVerticalAngle = -90f;

        [Tooltip("Maximum rotation of the arms and camera on the axis."),
         SerializeField]
        private float maxVerticalAngle = 90f;

        public Animator anim;

        [Tooltip("The names of the axes and buttons for Unity's Input Manager."), SerializeField]
        private FpsInput input;
#pragma warning restore 649

        public AutomaticGunScriptLPFP auto;
        private Rigidbody _rigidbody;
        private CapsuleCollider _collider;
        private AudioSource _audioSource;
        private SmoothRotation _rotationX;
        private SmoothRotation _rotationY;
        private SmoothVelocity _velocityX;
        private SmoothVelocity _velocityZ;
        private bool _isGrounded;
        public Camera camera;
        [SyncVar]
        public int playerId;

        private readonly RaycastHit[] _groundCastResults = new RaycastHit[8];
        private readonly RaycastHit[] _wallCastResults = new RaycastHit[8];

        public override void OnStartServer()
        {
            base.OnStartServer();
            int max = 0;
            foreach (FpsControllerLPFP fPS in FindObjectsOfType<FpsControllerLPFP>())
            {
                if(fPS.playerId > max)
                {
                    max = fPS.playerId;
                }
            }
            playerId = max + 1;
            /*FindObjectOfType<TankPlayerController>().GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
            Debug.Log("spawnteammates");
            TeamateSpawner.instance.SpawnTeamates(playerId);*/
        }
        public override void OnStopServer()
        {
            base.OnStopServer();
            Debug.Log("server stoppped");
            Firebase firebase = Firebase.CreateNew("https://heistday-9d49b-default-rtdb.firebaseio.com/");
            firebase.Child("mailbox").Child(RespawnPain.instance.docID).Delete();
        }
        /// Initializes the FpsController on start.
        public void Start()
        {
            _rotationX = new SmoothRotation(RotationXRaw);
            _rotationY = new SmoothRotation(RotationYRaw);
            _velocityX = new SmoothVelocity();
            _velocityZ = new SmoothVelocity();
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _collider = GetComponent<CapsuleCollider>();
            _audioSource = GetComponent<AudioSource>();
            if (!isLocalPlayer)
            {
                return;
            }
            
           
			arms = AssignCharactersCamera();
            _audioSource.clip = walkingSound;
            _audioSource.loop = true;
            Cursor.lockState = CursorLockMode.Locked;
            ValidateRotationRestriction();
            camera.gameObject.SetActive(true);
            if (!isLocalPlayer)
            {
                return;
            }
            MirrorVariables.instance.conn = connectionToServer;
        }
			
        private Transform AssignCharactersCamera()
        {
            var t = transform;
			arms.SetPositionAndRotation(t.position, t.rotation);
			return arms;
        }
        
        /// Clamps <see cref="minVerticalAngle"/> and <see cref="maxVerticalAngle"/> to valid values and
        /// ensures that <see cref="minVerticalAngle"/> is less than <see cref="maxVerticalAngle"/>.
        private void ValidateRotationRestriction()
        {
            minVerticalAngle = ClampRotationRestriction(minVerticalAngle, -90, 90);
            maxVerticalAngle = ClampRotationRestriction(maxVerticalAngle, -90, 90);
            if (maxVerticalAngle >= minVerticalAngle) return;
            Debug.LogWarning("maxVerticalAngle should be greater than minVerticalAngle.");
            var min = minVerticalAngle;
            minVerticalAngle = maxVerticalAngle;
            maxVerticalAngle = min;
        }

        private static float ClampRotationRestriction(float rotationRestriction, float min, float max)
        {
            if (rotationRestriction >= min && rotationRestriction <= max) return rotationRestriction;
            var message = string.Format("Rotation restrictions should be between {0} and {1} degrees.", min, max);
            Debug.LogWarning(message);
            return Mathf.Clamp(rotationRestriction, min, max);
        }
			
        /// Checks if the character is on the ground.
        private void OnCollisionStay()
        {
            var bounds = _collider.bounds;
            var extents = bounds.extents;
            var radius = extents.x - 0.01f;
            Physics.SphereCastNonAlloc(bounds.center, radius, Vector3.down,
                _groundCastResults, extents.y - radius * 0.5f, ~0, QueryTriggerInteraction.Ignore);
            if (!_groundCastResults.Any(hit => hit.collider != null && hit.collider != _collider)) return;
            for (var i = 0; i < _groundCastResults.Length; i++)
            {
                _groundCastResults[i] = new RaycastHit();
            }

            _isGrounded = true;
        }
			
        /// Processes the character movement and the camera rotation every fixed framerate frame.
        private void FixedUpdate()
        {
            if (!isLocalPlayer)
            {
                return;
            }
            ClientInput clientInput = new ClientInput(input);
            // FixedUpdate is used instead of Update because this code is dealing with physics and smoothing.
            if (isServer)
            {
                FixedUpdateFunction(false, clientInput);
            }
            else
            {
                CmdFixedUpdate(clientInput);
            }
        }
        [Command]
        private void CmdFixedUpdate(ClientInput clientInput)
        {
            FixedUpdateFunction(true, clientInput);
        }
		private void FixedUpdateFunction(bool callingFromClient, ClientInput clientInput)
        {
            MoveCharacter(callingFromClient, clientInput);
            _isGrounded = false;
            RotateCameraAndCharacter(callingFromClient, clientInput);
            
        }
        /// Moves the camera to the character, processes jumping and plays sounds every frame.
        private void Update()
        {
            /*if (!isLocalPlayer)
            {
                gameObject.GetComponent<Camera>().enabled = false;
                gameObject.GetComponent<AudioListener>().enabled = false;
            }*/
            if (!isLocalPlayer)
            {
                return;
            }
            ClientInput clientInput = new ClientInput(input);
            if (isServer)
            {
                UpdateFunction(false, clientInput);
            }
            else
            {
                CmdUpdate(clientInput);
            }
        }
        [Command]
        private void CmdUpdate(ClientInput clientInput)
        {
            UpdateFunction(true, clientInput);
        }
        private void UpdateFunction(bool callingFromClient, ClientInput clientInput)
        {
            arms.position = transform.position + transform.TransformVector(armPosition);
            Jump(callingFromClient, clientInput);
            PlayFootstepSounds(callingFromClient, clientInput);
        }

        private void RotateCameraAndCharacter(bool callingFromClient, ClientInput clientInput)
        {
            float rotXCurrent = _rotationX._current;
            float rotYCurrent = _rotationY._current;
            var rotationX = _rotationX.Update(clientInput.rotateX * mouseSensitivity, rotationSmoothness);
            var rotationY = _rotationY.Update(clientInput.rotateY * mouseSensitivity, rotationSmoothness);
            var clampedY = RestrictVerticalRotation(rotationY);
            _rotationY.Current = clampedY;
			var worldUp = arms.InverseTransformDirection(Vector3.up);
			var rotation = arms.rotation *
                           Quaternion.AngleAxis(rotationX, worldUp) *
                           Quaternion.AngleAxis(clampedY, Vector3.left);
            transform.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
			arms.rotation = rotation;
            if (callingFromClient)
            {
                Debug.Log("rotXCurrent " + rotXCurrent);
                Debug.Log("rotYCurrent " + rotYCurrent);
                Debug.Log("rotXTarget " + clientInput.rotateX * mouseSensitivity);
                Debug.Log("rotYTarget " + clientInput.rotateY * mouseSensitivity);
                Debug.Log("rotationX " + rotationX);
                Debug.Log("rotationY " + rotationY);
                Debug.Log("clampedY " + clampedY);
                Debug.Log("worldUp " + worldUp);
                Debug.Log("rotation.eulerAngles " + rotation.eulerAngles);
                Debug.Log("armRot " + arms.rotation.eulerAngles);
                Debug.Log("Rot1 " + arms.rotation * Quaternion.AngleAxis(rotationX, worldUp));
                Debug.Log("Rot2 " + new Vector3(0f, rotation.eulerAngles.y, 0f));
                Debug.Log("Transform.Eulerangles " + transform.eulerAngles);
            }
        }
			
        /// Returns the target rotation of the camera around the y axis with no smoothing.
        private float RotationXRaw
        {
            get { return input.RotateX * mouseSensitivity; }
        }
			
        /// Returns the target rotation of the camera around the x axis with no smoothing.
        private float RotationYRaw
        {
            get { return input.RotateY * mouseSensitivity; }
        }
			
        /// Clamps the rotation of the camera around the x axis
        /// between the <see cref="minVerticalAngle"/> and <see cref="maxVerticalAngle"/> values.
        private float RestrictVerticalRotation(float mouseY)
        {
			var currentAngle = NormalizeAngle(arms.eulerAngles.x);
            var minY = minVerticalAngle + currentAngle;
            var maxY = maxVerticalAngle + currentAngle;
            return Mathf.Clamp(mouseY, minY + 0.01f, maxY - 0.01f);
        }
			
        /// Normalize an angle between -180 and 180 degrees.
        /// <param name="angleDegrees">angle to normalize</param>
        /// <returns>normalized angle</returns>
        private static float NormalizeAngle(float angleDegrees)
        {
            while (angleDegrees > 180f)
            {
                angleDegrees -= 360f;
            }

            while (angleDegrees <= -180f)
            {
                angleDegrees += 360f;
            }

            return angleDegrees;
        }

        private void MoveCharacter(bool callingFromClient, ClientInput clientInput)
        {
            var direction = new Vector3(clientInput.move, 0f, clientInput.strafe).normalized;
            var worldDirection = transform.TransformDirection(direction);
            var velocity = worldDirection * (clientInput.run ? runningSpeed : walkingSpeed);
            //Checks for collisions so that the character does not stuck when jumping against walls.
            var intersectsWall = CheckCollisionsWithWalls(velocity);
            if (intersectsWall)
            {
                _velocityX.Current = _velocityZ.Current = 0f;
                return;
            }
            var rigidbodyVelocity = _rigidbody.velocity;
            _velocityX.Current = rigidbodyVelocity.x;
            _velocityZ.Current = rigidbodyVelocity.z;
            Vector2 current = new Vector2(_velocityX._current, _velocityZ._current);
            var smoothX = _velocityX.Update(velocity.x, movementSmoothness);
            var smoothZ = _velocityZ.Update(velocity.z, movementSmoothness);
            //var force = new Vector3(smoothX - rigidbodyVelocity.x, 0f, smoothZ - rigidbodyVelocity.z);
            //var force = new Vector3(velocity.x - rigidbodyVelocity.x, 0f, velocity.z - rigidbodyVelocity.z);
            //_rigidbody.AddForce(force, ForceMode.VelocityChange);
            _rigidbody.velocity = new Vector3(velocity.x, _rigidbody.velocity.y, velocity.z);
            if (callingFromClient)
            {
                //Debug.Log("force " + force);
                Debug.Log("smoothX " + smoothX);
                Debug.Log("smoothZ " + smoothZ);
                Debug.Log("rigidbodyvelocity " + rigidbodyVelocity);
                Debug.Log("worlddirection " + worldDirection);
                Debug.Log("input.Run " + clientInput.run);
                Debug.Log("walkingspeed " + walkingSpeed);
                Debug.Log("runningspeed " + runningSpeed);
                Debug.Log("Velocity" + velocity);
                Debug.Log("Current" + current + " " + new Vector2(smoothX, smoothZ));
                Debug.Log("MovementSmoothness" + movementSmoothness);

            }
            //Debug.Log("SmoothZBear " + smoothZ);
        }

        private bool CheckCollisionsWithWalls(Vector3 velocity)
        {
            if (_isGrounded) return false;
            var bounds = _collider.bounds;
            var radius = _collider.radius;
            var halfHeight = _collider.height * 0.5f - radius * 1.0f;
            var point1 = bounds.center;
            point1.y += halfHeight;
            var point2 = bounds.center;
            point2.y -= halfHeight;
            Physics.CapsuleCastNonAlloc(point1, point2, radius, velocity.normalized, _wallCastResults,
                radius * 0.04f, ~0, QueryTriggerInteraction.Ignore);
            var collides = _wallCastResults.Any(hit => hit.collider != null && hit.collider != _collider);
            if (!collides) return false;
            for (var i = 0; i < _wallCastResults.Length; i++)
            {
                _wallCastResults[i] = new RaycastHit();
            }

            return true;
        }

        private void Jump(bool callingFromClient, ClientInput clientInput)
        {
            if (!_isGrounded || !clientInput.jump) return;
            _isGrounded = false;
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (callingFromClient)
            {
                Debug.Log("JumpForce " + jumpForce);
                Debug.Log("Vector3.up " + Vector3.up);
                Debug.Log("rigidbodyvelocity " + _rigidbody.velocity);
            }
            //Debug.Log("JumpForce " + jumpForce);
            //Debug.Log("Vector3.up " + Vector3.up);
            //Debug.Log("rigidbodyvelocity " + _rigidbody.velocity);
        }
        [Command]
        public void CmdSetBool(string name, bool tf)
        {
            anim.SetBool(name, tf);
        }
        [Command]
        public void CmdSetTrigger(string name)
        {
            anim.SetTrigger(name);
        }
        [Command]
        public void CmdPlay(string name, int i, float f)
        {
            anim.Play(name, i, f);
        }

        private void PlayFootstepSounds(bool callingFromClient, ClientInput clientInput)
        {
            if (_isGrounded && _rigidbody.velocity.sqrMagnitude > 0.1f)
            {
                _audioSource.clip = clientInput.run ? runningSound : walkingSound;
                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
            }
            else
            {
                if (_audioSource.isPlaying)
                {
                    _audioSource.Pause();
                }
            }
        }
        public bool IsLocalPlayer()
        {
            return isLocalPlayer;
        }
			
        /// A helper for assistance with smoothing the camera rotation.
        public class SmoothRotation
        {
            public float _current;
            public float _currentVelocity;

            public SmoothRotation(float startAngle)
            {
                _current = startAngle;
            }
				
            /// Returns the smoothed rotation.
            public float Update(float target, float smoothTime)
            {
                return _current = Mathf.SmoothDampAngle(_current, target, ref _currentVelocity, smoothTime);
            }

            public float Current
            {
                set { _current = value; }
            }
        }
			
        /// A helper for assistance with smoothing the movement.
        public class SmoothVelocity
        {
            public float _current;
            public float _currentVelocity;

            /// Returns the smoothed velocity.
            public float Update(float target, float smoothTime)
            {
                return _current = Mathf.SmoothDamp(_current, target, ref _currentVelocity, smoothTime);
            }

            public float Current
            {
                set { _current = value; }
            }
        }

        [Command(ignoreAuthority = true)]
        public void CmdShootBullet()
        {
            auto.ShootBulletOnServer();
        }
        [Command(ignoreAuthority = true)]
        public void CmdThrowGrenade()
        {
            auto.SpawnGrenadeOnServer();
        }

        /// Input mappings
        [Serializable]
        public class FpsInput
        {
            [Tooltip("The name of the virtual axis mapped to rotate the camera around the y axis."),
             SerializeField]
            private string rotateX = "Mouse X";

            [Tooltip("The name of the virtual axis mapped to rotate the camera around the x axis."),
             SerializeField]
            private string rotateY = "Mouse Y";

            [Tooltip("The name of the virtual axis mapped to move the character back and forth."),
             SerializeField]
            private string move = "Horizontal";

            [Tooltip("The name of the virtual axis mapped to move the character left and right."),
             SerializeField]
            private string strafe = "Vertical";

            [Tooltip("The name of the virtual button mapped to run."),
             SerializeField]
            private string run = "Fire3";

            [Tooltip("The name of the virtual button mapped to jump."),
             SerializeField]
            private string jump = "Jump";

            /// Returns the value of the virtual axis mapped to rotate the camera around the y axis.
            public float RotateX
            {
                get { return Input.GetAxisRaw(rotateX); }
            }
				         
            /// Returns the value of the virtual axis mapped to rotate the camera around the x axis.        
            public float RotateY
            {
                get { return Input.GetAxisRaw(rotateY); }
            }
				        
            /// Returns the value of the virtual axis mapped to move the character back and forth.        
            public float Move
            {
                get { return Input.GetAxisRaw(move); }
            }
				       
            /// Returns the value of the virtual axis mapped to move the character left and right.         
            public float Strafe
            {
                get { return Input.GetAxisRaw(strafe); }
            }
				    
            /// Returns true while the virtual button mapped to run is held down.          
            public bool Run
            {
                get { return Input.GetButton(run); }
            }
				     
            /// Returns true during the frame the user pressed down the virtual button mapped to jump.          
            public bool Jump
            {
                get { return Input.GetButtonDown(jump); }
            }
        }
        public class ClientInput
        {
            public float rotateX, rotateY, move, strafe;
            public bool run, jump;

            public ClientInput()
            {

            }


            public ClientInput(FpsInput input)
            {
                rotateX = input.RotateX;
                rotateY = input.RotateY;
                move = input.Move;
                strafe = input.Strafe;
                run = input.Run;
                jump = input.Jump;
        

      }
        }
    }
}