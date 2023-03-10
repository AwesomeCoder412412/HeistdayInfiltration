using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
public class RoomProcGen : MonoBehaviour
{
    //TODO:ui indication of when u move on to the next room
    public Prop[] props;
    public GameObject floor;
    public GameObject gravityPlatform;
    public float roomZ, roomWidth, roomHeight;
    public RoomType roomType;
    public Vector3 platOffset;
    public Vector2 platformSpawnableDistance;
    public int maxPlatOffset, minPlatOffset;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Prop prop in props)
        {
            for (int i = 0; i < Random.Range(prop.min, prop.max); i++)
            {
                float maxDimension = Mathf.Pow((Mathf.Pow(prop.prop.width, 2) + Mathf.Pow(prop.prop.height, 2)), 0.5f) / 2;
                Vector3 randomPos = floor.transform.position + new Vector3(roomWidth / 2, 2, roomZ / 2) + new Vector3(Random.Range((-(roomWidth) / 2) + maxDimension, (roomWidth / 2) - maxDimension), 0, Random.Range((-(roomZ) / 2) + maxDimension, (roomZ / 2) - maxDimension));
                Vector3 randomRot = new Vector3(0, Random.Range(0, 360), 0);
                GameObject boxInstant = Instantiate(prop.prop.gameObject, transform);
                boxInstant.transform.rotation = Quaternion.Euler(randomRot);
                boxInstant.transform.position = randomPos;
            }
        }
        if (roomType == RoomType.TIMED_ESCAPE_GRAVITY)
        {
            GameObject[] platforms = new GameObject[10];
            Vector3[] gravities = getNewGravs();
            int iterations = 0;
            int limit = Random.Range(10, 10);
            int i = 0;
            for (i = 0; i < limit && iterations < 1000; i++)
            {
                GameObject platform = Instantiate(gravityPlatform, transform);
                platforms[i] = platform;
                if (i == 0)
                {
                    platform.transform.position = floor.transform.position + platOffset;
                    //replaced random range 1 5 with 0 gravities[Random.Range(1,5)]
                    platform.GetComponent<GravityPlatform>().SetGravity(gravities[Random.Range(1, 5)]);

                }
                else
                {

                    GameObject previousPlat = platforms[i - 1];

                    Vector3 prevGrav = (Vector3)previousPlat.GetComponent<GravityPlatform>().gravity;
                    Vector3 ppGrav = i >= 2 ? (Vector3)platforms[i - 2].GetComponent<GravityPlatform>().gravity : Vector3.down;
                    bool validPos = false;

                    Vector3 targetRot = Vector3.zero;
                    if (prevGrav.x != 0)
                    {
                        targetRot.z = (prevGrav.x > 0 ? 90 : 270);

                    }
                    else if (prevGrav.y != 0)
                    {
                        targetRot.z = (prevGrav.y > 0 ? 180 : 0);
                    }
                    else
                    {
                        targetRot.x = (prevGrav.z > 0 ? 270 : 90);
                    }
                    platform.transform.rotation = Quaternion.Euler(targetRot);
                    Vector3 middleFloor = new Vector3(floor.transform.position.x + roomWidth / 2, floor.transform.position.y, floor.transform.position.z + roomZ / 2);
                    Debug.Log("minY floor " + middleFloor.y + "( " + targetRot.x + ", " + targetRot.z + ")");
                    float minX = middleFloor.x - platformSpawnableDistance.x / 2 + (targetRot.z == 0 && targetRot.y == 0 ? 6 : 6);//replaced 0 with 6
                    float maxX = middleFloor.x + platformSpawnableDistance.x / 2 - (targetRot.z == 0 && targetRot.y == 0 ? 6 : 6);//replaced 0 with 6
                    float minY = middleFloor.y + (targetRot.x == 0 && targetRot.z == 0 ? 6 : 6);//replaced 0 with 6
                    float maxY = middleFloor.y + roomHeight - (targetRot.x == 0 && targetRot.z == 0 ? 6 : 6);//replaced 0 with 6
                    float minZ = middleFloor.z - platformSpawnableDistance.y / 2 + (targetRot.x == 0 && targetRot.y == 0 ? 6 : 6);//replaced 0 with 6
                    float maxZ = middleFloor.z + platformSpawnableDistance.y / 2 - (targetRot.x == 0 && targetRot.y == 0 ? 6 : 6); //replaced 0 with 6
                    float lineY = floor.transform.position.y + roomHeight / 2;
                    //DrawLine(new Vector3(minX, lineY, minZ), new Vector3(minX, lineY, maxZ));
                    //DrawLine(new Vector3(maxX, lineY, minZ), new Vector3(maxX, lineY, maxZ));
                    //DrawLine(new Vector3(minX, lineY, maxZ), new Vector3(maxX, lineY, maxZ));
                    //DrawLine(new Vector3(minX, lineY, minZ), new Vector3(maxX, lineY, minZ));
                    //changed 30 to 1
                    List<int> list = Enumerable.Range(20, 30).ToList();
                    list = list.OrderBy(x => Random.value).ToList();
                    List<int> offsetList = Enumerable.Range(minPlatOffset, maxPlatOffset).ToList();
                    Debug.Log("offsets: min:" + minPlatOffset + " max: " + maxPlatOffset + " length: " + offsetList.Count());
                    offsetList = offsetList.OrderBy(x => Random.value).ToList();
                    for (int j = 0; j < list.Count && !validPos; j++)
                    {
                       // Debug.Log("jloop " + list.Count + " validPos " + validPos);
                        int g = list[j];
                        Vector3[] offsetDirs;
                        if (prevGrav.x != 0)
                        {
                            offsetDirs = new Vector3[] { Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
                        }
                        else if (prevGrav.y != 0)
                        {
                            offsetDirs = new Vector3[] { Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
                        }
                        else
                        {
                            offsetDirs = new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
                        }
                        offsetDirs = offsetDirs.OrderBy(x => Random.value).ToArray();
                        //changed 4 to 1
                        for (int k = 0; k < 4 && !validPos; k++)
                        {
                           // Debug.Log("kloop " + k + " validPos " + validPos);
                            Vector3 offsetDir = offsetDirs[k];
                            //changed offsetList.Count to 1
                            for (int l = 0; l < 1 && !validPos; l++)
                            {
                                //Debug.Log("lloop " + offsetList.Count + " validPos " + validPos);
                                int offset = offsetList[l];
                                Vector3 newPos = previousPlat.transform.position + prevGrav.normalized * g + offsetDir * offset;
                                Vector3 oldPos = newPos;
                                Debug.Log(" prevPlat: " + previousPlat.transform.position + " prevGrav: " + prevGrav.normalized + " g: " + g + " offsetDir: " + offsetDir +  " offset: " + offset + " newPos: " + newPos); 
                                if (ppGrav.x != 0 && prevGrav.x == 0)
                                {
                                    newPos.x -= 6 * Mathf.Sign(ppGrav.x);
                                }
                                else if (ppGrav.y != 0 && prevGrav.y == 0)
                                {
                                    newPos.y -= 6 * Mathf.Sign(ppGrav.y);
                                }
                                else if (ppGrav.z != 0 && prevGrav.z == 0)
                                {
                                    newPos.z -= 6 * Mathf.Sign(ppGrav.z);
                                }
                                platform.transform.position = newPos;
                                if (newPos.x > minX && newPos.x < maxX && newPos.y > minY && newPos.y < maxY && newPos.z > minZ && newPos.z < maxZ)
                                {
                                    validPos = true;
                                    for (int q = 0; q <= i - 2 && validPos; q++)
                                    {
                                        GameObject firstPlat = platforms[q];
                                        GameObject secondPlat = platforms[q + 1];
                                        if (checkPlatformIntersection(firstPlat, secondPlat, platform))
                                        {
                                            Debug.Log("intersection1 " + q + " " + firstPlat.name + ", " + secondPlat.name);
                                            validPos = false;
                                        }
                                    }
                                    for (int q = 0; q <= i - 2 && validPos; q++)
                                    {
                                        GameObject platToCheck = platforms[q];
                                        if (checkPlatformIntersection(platforms[i - 1], platform, platToCheck))
                                        {
                                            Debug.Log("intersection2 " + q + " " + platToCheck.name + ", " + platforms[i - 1].name);
                                            validPos = false;
                                        }
                                    }
                                }
                                platform.name = "GravityPlatform(Clone) i: " + i + " newPos " + newPos + " oldPos " + oldPos;
                                Debug.Log(platform.name + " validPos " + validPos + " g " + g + " newPos1 " + newPos + " minX " + minX + " maxX " + maxX + " minY " + minY + " maxY " + maxY +
                                    " minZ " + minZ + " maxZ " + maxZ + " " + (newPos.x > minX) + " " + (newPos.x < maxX) + " " + (newPos.y > minY) + " " + (newPos.y < maxY) + " " + (newPos.z > minZ) + " "
                                    + (newPos.z < maxZ) + " ppGrav " + ppGrav + " prevGrav " + prevGrav + " middleFloor " + middleFloor + " targetRot " + targetRot + " platformSpawnableDistance " + platformSpawnableDistance + "platforms length " + platforms.Length);
                            }
                         //   Debug.Log("endlloop " + offsetList.Count + " validPos " + validPos);
                        }
                    }
                    GameObject platformToSetGravity = null;
                    Vector3 gravCompare;
                    if (validPos)
                    {
                        platformToSetGravity = platform;
                        gravCompare = prevGrav;
                        gravities = getNewGravs();
                        gravities = gravities.Where(gravity =>
                        {
                            if (gravity.x > 0 && gravCompare.x > 0)
                            {
                                return false;
                            }
                            else if (gravity.y > 0 && gravCompare.y > 0)
                            {
                                return false;
                            }
                            else if (gravity.z > 0 && gravCompare.z > 0)
                            {
                                return false;
                            }
                            else if (gravity.x < 0 && gravCompare.x < 0)
                            {
                                return false;
                            }
                            else if (gravity.y < 0 && gravCompare.y < 0)
                            {
                                return false;
                            }
                            else if (gravity.z < 0 && gravCompare.z < 0)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }

                        }).ToArray();
                    }
                    else
                    {
                        gravCompare = prevGrav;
                        platformToSetGravity = previousPlat;
                        platforms[i] = null;
                        i--;
                        Destroy(platform);
                        gravities = gravities.Where(gravity =>
                        {
                            if (gravity.x > 0 && gravCompare.x > 0)
                            {
                                return false;
                            }
                            else if (gravity.y > 0 && gravCompare.y > 0)
                            {
                                return false;
                            }
                            else if (gravity.z > 0 && gravCompare.z > 0)
                            {
                                return false;
                            }
                            else if (gravity.x < 0 && gravCompare.x < 0)
                            {
                                return false;
                            }
                            else if (gravity.y < 0 && gravCompare.y < 0)
                            {
                                return false;
                            }
                            else if (gravity.z < 0 && gravCompare.z < 0)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }

                        }).ToArray();
                    }

                    gravities = gravities.OrderBy(x => Random.value).ToArray();
                    if (gravities.Length > 0)
                    {
                        platformToSetGravity.GetComponent<GravityPlatform>().SetGravity(gravities[0]);
                    }
                    else
                    {
                        iterations = 999999;
                    }
                    Debug.Log("iterations " + iterations + " i " + i + " length " + gravities.Length + " gravCompare " + gravCompare + " " + (gravities.Length > 0 ? gravities[0].ToString() : "") + " "
                        + (gravities.Length > 1 ? gravities[1].ToString() : "") + " " + (gravities.Length > 2 ? gravities[2].ToString() : "") + " " + (gravities.Length > 3 ? gravities[3].ToString() : "") +
                        " " + (gravities.Length > 4 ? gravities[4].ToString() : "") + " " + (gravities.Length > 5 ? gravities[5].ToString() : ""));
                }
                iterations++;


            }
            platforms[i - 1].tag = "Fridge";
            for (int w = 0; w < platforms.Length - 2; w++)
            {
                if (platforms[w] && platforms[w + 1])
                {
                    drawBounds(platforms[w], platforms[w + 1]);
                }
            }
        }

    }

    public Vector3[] getNewGravs()
    {
        Vector3[] gravities = new Vector3[6];
        gravities[0] = Vector3.down;
        gravities[1] = Vector3.up;
        gravities[2] = Vector3.right;
        gravities[3] = Vector3.left;
        gravities[4] = Vector3.forward;
        gravities[5] = Vector3.back;
        return gravities;
    }

    public void drawBounds(GameObject firstPlat, GameObject secondPlat)
    {
        //Vector3 grav = (Vector3)firstPlat.GetComponent<GravityPlatform>().gravity;
        //Vector3 plat2TR = FindPlatformBounds(1, 1, secondPlat);
        //Vector3 plat2TL = FindPlatformBounds(-1, 1, secondPlat);
        //Vector3 plat2BR = FindPlatformBounds(1, -1, secondPlat);
        //Vector3 plat2BL = FindPlatformBounds(-1, -1, secondPlat);
        //Vector3 plat1TR = FindPlatformBounds2(firstPlat, plat2TR, grav);
        //Vector3 plat1TL = FindPlatformBounds2(firstPlat, plat2TL, grav);
        //Vector3 plat1BR = FindPlatformBounds2(firstPlat, plat2BR, grav);
        //Vector3 plat1BL = FindPlatformBounds2(firstPlat, plat2BL, grav);
        //Debug.Log("plat1: name: " + firstPlat.name + " grav " + grav + " start of bounds: " + plat1TR + " " + plat1TL + " " + plat1BR + " " + plat1BL + " plat2: " + plat2TR + " " + plat2TL + " " + plat2BR + " " + plat2BL);
        Vector3[] corners = findCorners(firstPlat, secondPlat);
        Vector3 plat2TR = corners[4];
        Vector3 plat2TL = corners[5];
        Vector3 plat2BR = corners[7];
        Vector3 plat2BL = corners[6];
        Vector3 plat1TR = corners[0];
        Vector3 plat1TL = corners[1];
        Vector3 plat1BR = corners[3];
        Vector3 plat1BL = corners[2];
        DrawLine(plat2TR, plat2TL);
        DrawLine(plat2TL, plat2BL);
        DrawLine(plat2BL, plat2BR);
        DrawLine(plat2BR, plat2TR);
        DrawLine(plat2TR, plat1TR);
        DrawLine(plat2TL, plat1TL);
        DrawLine(plat2BL, plat1BL);
        DrawLine(plat2BR, plat1BR);
        DrawLine(plat1TR, plat1TL);
        DrawLine(plat1TL, plat1BL);
        DrawLine(plat1BL, plat1BR);
        DrawLine(plat1BR, plat1TR);
    }

    public bool checkPlatformIntersection(GameObject firstPlat, GameObject secondPlat, GameObject platform)
    {
        //Vector3 grav = (Vector3)firstPlat.GetComponent<GravityPlatform>().gravity;
        //Vector3 plat2TR = FindPlatformBounds(1, 1, secondPlat);
        //Vector3 plat2TL = FindPlatformBounds(-1, 1, secondPlat);
        //Vector3 plat2BR = FindPlatformBounds(1, -1, secondPlat);
        //Vector3 plat2BL = FindPlatformBounds(-1, -1, secondPlat);
        //Vector3 plat1TR = FindPlatformBounds2(firstPlat, plat2TR, grav);
        //Vector3 plat1TL = FindPlatformBounds2(firstPlat, plat2TL, grav);
        //Vector3 plat1BR = FindPlatformBounds2(firstPlat, plat2BR, grav);
        //Vector3 plat1BL = FindPlatformBounds2(firstPlat, plat2BL, grav);
        //Debug.Log("plat1: " + plat1TR + " " + plat1TL + " " + plat1BR + " " + plat1BL + " plat2: " + plat2TR + " " + plat2TL + " " + plat2BR + " " + plat2BL);
        Vector3[] corners = findCorners(firstPlat, secondPlat);
        return intersects(corners[0], corners[1], corners[2], corners[3], corners[4], corners[5], corners[6], corners[7], platform);
        
    } 

    public Vector3 FindPlatformBounds(int x, int y, GameObject plat)
    {
        
        Vector3 output = plat.transform.position;
        if (plat.transform.eulerAngles.z == 90 || plat.transform.eulerAngles.z == 270)
        {
            output += Vector3.up * 6 * y + Vector3.forward * 6 * x;

        }
        //z = 0 satisfying multiple cases
        else if (plat.transform.eulerAngles.x == 90 || plat.transform.eulerAngles.x == 270)
        {
            output += Vector3.up * 6 * y + Vector3.right * 6 * x;

        }
        else if (plat.transform.eulerAngles.z == 0 || plat.transform.eulerAngles.z == 180)
        {
            output += Vector3.forward * 6 * y + Vector3.right * 6 * x;

        }
        return output;
    }
    public Vector3 FindPlatformBounds2(GameObject plat, Vector3 coord, Vector3 grav)
    {
        //Debug.Log("FPB2 name: " + plat.name + " euler " + plat.transform.eulerAngles + " coord " + coord + " grav " + grav + " pos " + plat.transform.position);
        if (plat.transform.eulerAngles.z == 90 || plat.transform.eulerAngles.z == 270)
        {
            if (grav.x != 0)
            {
                coord.x = plat.transform.position.x;
            }
            else if (grav.y != 0)
            {
                coord.y = plat.transform.position.y - 6 * Mathf.Sign(grav.y);
            }
            else
            {
                coord.z = plat.transform.position.z - 6 * Mathf.Sign(grav.z);
            }

        }
        else if (plat.transform.eulerAngles.x == 90 || plat.transform.eulerAngles.x == 270)
        {
            if (grav.x != 0)
            {
                coord.x = plat.transform.position.x - 6 * Mathf.Sign(grav.x);
            }
            else if (grav.y != 0)
            {
                coord.y = plat.transform.position.y - 6 * Mathf.Sign(grav.y);
            }
            else
            {
                coord.z = plat.transform.position.z;
            }

        }
        else if (plat.transform.eulerAngles.z == 0 || plat.transform.eulerAngles.z == 180)
        {
            if (grav.x != 0)
            {
                coord.x = plat.transform.position.x - 6 * Mathf.Sign(grav.x);
            }
            else if (grav.y != 0)
            {
                coord.y = plat.transform.position.y;
            }
            else
            {
                coord.z = plat.transform.position.z - 6 * Mathf.Sign(grav.z);
            }

        }
        return coord;
    }
    public void DrawLine(Vector3 pos1, Vector3 pos2)
    {
        Debug.DrawRay(pos1, pos2 - pos1, Color.red, 99999);
        //Debug.Log("ray drawn, pos1: " + pos1 + " pos2: " + pos2);
    }
    //need to find corresponding corners, 2 corresponding corners would be a switch of one of them from min to max
    public bool intersects(Vector3 TR1, Vector3 TL1, Vector3 BL1, Vector3 BR1, Vector3 TR2, Vector3 TL2, Vector3 BL2, Vector3 BR2, GameObject plat)
    {
        
        Vector3[][] correspondingCorners = new Vector3[][] { new Vector3[] { TR1, TL1 }, new Vector3[] { BR1, BL1 }, new Vector3[] { TR1, BR1 }, new Vector3[] {TL1, BL1}, new Vector3[] { TR2, TL2 }, new Vector3[] { BR2, BL2 }, new Vector3[] { TR2, BR2 }, new Vector3[] { TL2, BL2 }, new Vector3[] { TL1, TL2 }, new Vector3[] { TR1, TR2 }, new Vector3[] { BR1, BR2 }, new Vector3[] { BL1, BL2 } };
        Vector3[] corners = new Vector3[] {plat.transform.position, FindPlatformBounds(1, 1, plat), FindPlatformBounds(1, -1, plat), FindPlatformBounds(-1, 1, plat), FindPlatformBounds(-1, -1, plat)};
        int count1 = 0;
        foreach(Vector3 coordPos in corners)
        {
            bool intersects = true;
            int count = 0;
            foreach(Vector3[] correspondingCorner in correspondingCorners)
            {
                
                if (intersects && !intersectsCorner(correspondingCorner[0], correspondingCorner[1], coordPos))
                {
                    intersects = false;
                    Debug.Log("middle: " + plat.transform.position + ", corner1: " + correspondingCorner[0] + ", corner2: " + correspondingCorner[1] + ", edge index: " + count + ", corner index: " + count1);
                }
                count++;
            }
            if (intersects)
            {
                Vector3 plat2TR = TR2;
                Vector3 plat2TL = TL2;
                Vector3 plat2BR = BR2;
                Vector3 plat2BL = BL2;
                Vector3 plat1TR = TR1;
                Vector3 plat1TL = TL1;
                Vector3 plat1BR = BR1;
                Vector3 plat1BL = BL1;
                DrawLine(plat2TR, plat2TL);
                DrawLine(plat2TL, plat2BL);
                DrawLine(plat2BL, plat2BR);
                DrawLine(plat2BR, plat2TR);
                DrawLine(plat2TR, plat1TR);
                DrawLine(plat2TL, plat1TL);
                DrawLine(plat2BL, plat1BL);
                DrawLine(plat2BR, plat1BR);
                DrawLine(plat1TR, plat1TL);
                DrawLine(plat1TL, plat1BL);
                DrawLine(plat1BL, plat1BR);
                DrawLine(plat1BR, plat1TR);
                Debug.DrawLine(coordPos, coordPos + Vector3.one * 0.5f, Color.blue, 999999);
                return true;
            }
            count1++;
        }
        return false;
    }

    public Vector3[] cornerDiff(Vector3 p1, Vector3 p2)
    {
        if (p1.x != p2.x)
        {
            return new Vector3[] {new Vector3(p1.x, 0, 0), new Vector3(p2.x, 0, 0) };
        }
        if (p1.y != p2.y)
        {
            return new Vector3[] {new Vector3(0, p1.y, 0), new Vector3(0, p2.y, 0)};
        }
        return new Vector3[] {new Vector3(0, 0, p1.z), new Vector3(0, 0, p2.z)};
    }

    public bool intersectsCorner(Vector3 corner1, Vector3 corner2, Vector3 pos)
    {
        Vector3[] cornerDiffs = cornerDiff(corner1, corner2);
        if (cornerDiffs[0].x != 0)
        {
            if (cornerDiffs[0].x > cornerDiffs[1].x)
            {
                if (cornerDiffs[0].x >= pos.x && cornerDiffs[1].x <= pos.x)
                {
                    return true;
                }
            }
            else if (cornerDiffs[0].x < cornerDiffs[1].x)
            {
                if (cornerDiffs[0].x <= pos.x && cornerDiffs[1].x >= pos.x)
                {
                    return true;
                }
            }
        }
        else if (cornerDiffs[0].y != 0)
        {
            if (cornerDiffs[0].y > cornerDiffs[1].y)
            {
                if (cornerDiffs[0].y >= pos.y && cornerDiffs[1].y <= pos.y)
                {
                    return true;
                }
            }
            else if (cornerDiffs[0].y < cornerDiffs[1].y)
            {
                if (cornerDiffs[0].y <= pos.y && cornerDiffs[1].y >= pos.y)
                {
                    return true;
                }
            }
        }
        else if (cornerDiffs[0].z != 0)
        {
            if (cornerDiffs[0].z > cornerDiffs[1].z)
            {
                if (cornerDiffs[0].z >= pos.z && cornerDiffs[1].z <= pos.z)
                {
                    return true;
                }
            }
            else if (cornerDiffs[0].z < cornerDiffs[1].z)
            {
                if (cornerDiffs[0].z <= pos.z && cornerDiffs[1].z >= pos.z)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public Vector3[] findCorners(GameObject plat1, GameObject plat2)
    {
        Vector3 plat2TR = FindPlatformBounds(1, 1, plat2);
        Vector3 plat2TL = FindPlatformBounds(-1, 1, plat2);
        Vector3 plat2BR = FindPlatformBounds(1, -1, plat2);
        Vector3 plat2BL = FindPlatformBounds(-1, -1, plat2);
        Vector3 plat1TR = FindPlatformBounds(1, 1, plat1);
        Vector3 plat1TL = FindPlatformBounds(-1, 1, plat1);
        Vector3 plat1BR = FindPlatformBounds(1, -1, plat1);
        Vector3 plat1BL = FindPlatformBounds(-1, -1, plat1);
        Vector3[] platBounds = new Vector3[] { plat2TR, plat2TL, plat2BR, plat2BL, plat1TR, plat1TL, plat1BR, plat1BL };
        float minX = plat2TR.x;
        float minY = plat2TR.y;
        float maxY = plat2TR.y;
        float maxX = plat2TR.x;
        float minZ = plat2TR.z;
        float maxZ = plat2TR.z;
        foreach (Vector3 platBound in platBounds) {
            if (platBound.x < minX)
            {
                minX = platBound.x;
            }
            if (platBound.y < minY)
            {
                minY = platBound.y;
            }
            if (platBound.z < minZ)
            {
                minZ = platBound.z;
            }
            if (platBound.z > maxZ)
            {
                maxZ = platBound.z;
            }
            if (platBound.x > maxX)
            {
                maxX = platBound.x;
            }
            if (platBound.y > maxY)
            {
                maxY = platBound.y;
            }
        }
        return new Vector3[] { new Vector3(maxX, maxY, maxZ), new Vector3(minX, maxY, maxZ), new Vector3(minX, minY, maxZ), new Vector3(maxX, minY, maxZ), new Vector3(maxX, maxY, minZ), new Vector3(minX, maxY, minZ), new Vector3(minX, minY, minZ), new Vector3(maxX, minY, minZ) };
      }


    // Update is called once per frame
    void Update()
    {
        
    }
}
[System.Serializable]
public class Prop
{
    public int min, max;
    public PropProp prop;



}