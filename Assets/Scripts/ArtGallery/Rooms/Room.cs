﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO This should be modified to dynamically build the room based on parameters set by ArtGallery. What about a lobby?
/// <summary>
/// The only room in the Art Gallery.
/// </summary>
public class Room : MonoBehaviour
{

    //private bool debug = ArtGallery.DEBUG_LEVEL < ArtGallery.DEBUG.NONE;
    private bool debug = ArtGallery.DEBUG_LEVEL > 0;
    public GameObject portalObject;
    public GameObject SculpturePlatformObject;
    //public GameObject sculptureObject;
    public GameObject functionPickupObject;

    SortedList<int, Portal> portals; // Portal index is door index

    int NUM_WALLS = 4;
    int NUM_PORTALS = 4;
    float xSpacing = 9.9f;
    float ySpacing = 2.5f;
    float zSpacing = 9.9f;

    // portal lockout
    public bool Locked { get; set; }

    //private int rewindPortalID; // Parent of this room
    private bool isPopulated = false;  // is this room initialized?

    private RoomConfiguration roomController;
    
    /*
     * MOVE THIS TO PORTAL
    private Texture2D[] images;
    */
    
    private ArtGallery ag;

    private SculpturePlatform[] sculptures;

    public SculpturePlatform[] GetSculptures()
    {
        return sculptures;
    }

    /// <summary>
    /// Initial creation of the room
    /// </summary>
    public void InitializeRoom(RoomConfiguration _roomController)
    {
        ag = ArtGallery.GetArtGallery();
        roomController = _roomController;
        portals = new SortedList<int, Portal>();
        sculptures = new SculpturePlatform[4]; //HACK hardcoded values
        CreatePortals();
        CreateSculptures();
        isPopulated = true;
    }

    /* Public methods */

    public bool IsPopulated()
    {
        return isPopulated;
    }

    //create sculptures and position them
    //HACK hardcoded values and number of sculptures
    private void CreateSculptures()
    {
        Vector3[] sculps = new Vector3[] { new Vector3(-7.5f, 1.25f, -7.5f), new Vector3(7.5f, 1.25f, -7.5f), new Vector3(7.5f, 1.25f, 7.5f), new Vector3(-7.5f, 1.25f, 7.5f) };

        for (int i = 0; i < sculps.Length; i++)
        {
            GameObject sculpture = Instantiate(SculpturePlatformObject) as GameObject;
            sculpture.transform.SetParent(transform);
            sculpture.transform.position = sculps[i];
            //sculpture.AddComponent<Sculpture>();
            //sculpture.GetComponent<Sculpture>().VoxelObject = VoxelObject;
            //sculpture.GetComponent<Sculpture>().SculturePlatformObject = SculpturePlatformObject;

            if (UnityEngine.Random.Range(0, 1) < .5f) //HACK hardcoded transparency chance
            {
                //sculpture.GetComponent<Sculpture>().ToggleTransparency();
            }
            sculptures[i] = sculpture.GetComponent<SculpturePlatform>();
            sculptures[i].PortalID = i;
            sculptures[i].DestinationID = ((2 + i) % sculps.Length);
            sculpture.GetComponent<SculpturePlatform>().InitArtDisplay(roomController.GetRoomArt()[i + 4]);
        }

    }

    private void SpawnPickups()
    {
        FTYPE fTYPE = ActivationFunctions.GetRandom();

        if (UnityEngine.Random.Range(0f, 1f) < ag.functionSpawnRate) 
        {
            GameObject functionPickup = Instantiate(functionPickupObject) as GameObject;
            FunctionPickup fp = functionPickup.GetComponent<FunctionPickup>();
            SavedFunction sf = new SavedFunction
            {
                fTYPE = fTYPE
            };
            sf.GenerateThumbnail();
            fp.Function = sf;
        }
    }

    private void CreatePortals()
    {
        ArrayList walls = getWallObjects();
        int numImagesPerWall = 1;
        //used for portal id
        int idSet = 0;

        for (int i = 0; i < NUM_WALLS; i++)
        {
            for (int j = 1; j <= numImagesPerWall; j++)
            {
                Portal p = SpawnPortal(idSet);
                idSet++;
                if (true)//debug)
                {
                    //Debug.Log("received portal with ID " + p.GetPortalID());
                    if (debug) Debug.Log("Wall " + ((GameObject)walls[i]).name);
                }


                float tempZ = ((GameObject)walls[i]).transform.position.z / j;
                float tempY = ((GameObject)walls[i]).transform.position.y - 1.25f;
                float tempX = ((GameObject)walls[i]).transform.position.x / j;
                //Quaternion tempRot = ((GameObject)walls[i]).transform.rotation;

                if(debug) Debug.Log("Portal " + idSet + ":  X=" + tempX + "  Y=" + tempY +" Z=" + tempZ);

                //correctly position portal on wall
                Vector3 wallCenter = new Vector3(tempX, tempY, tempZ);
                Vector3 origin = new Vector3(0, 0, 0); // Center of room
                Vector3 fromWallTowardCenter = origin - wallCenter;
                Vector3 slightlyAwayFromWall = wallCenter + 0.005f * fromWallTowardCenter; // So that portal is not inside of wall
                // Places portal just in front of the wall
                p.transform.position = slightlyAwayFromWall;


                //correctly rotate portal on wall
                Vector3 noon = new Vector3(0, 0, 1);
                Vector3 other = new Vector3(slightlyAwayFromWall.x, 0, slightlyAwayFromWall.z);
                float angle = Vector3.SignedAngle(noon, other, noon);

                // BEWARE: Not sure if this special case is an ugly hack or a general solution.
                // The problem is that SignedAngle will return the smaller of the two possible rotation amounts.
                // So, in the case of a room with 4 walls, when the rotation should be 270, a result of 90 is computed (wrong direction).
                // This check fixes this issue, but will it break for more than 4 walls? The actual solution is probably close to this,
                // but we won't know until we test this with more walls.
                if (other.x < 0)
                {
                    angle = 360 - angle;
                }

                float rotAmount = angle - (360/NUM_WALLS);
                p.transform.Rotate(new Vector3(0, rotAmount, 0));

                //p.PaintDoor(images[i]);
            }
        }
    }

    private ArrayList getWallObjects()
    {
        ArrayList walls = new ArrayList();
        foreach (GameObject gameObj in FindObjectsOfType<GameObject>())
        {
            if (gameObj.tag == "wall")
            {
                walls.Add(gameObj);
            }
        }
        return walls;
    }

    private Portal SpawnPortal(int portalID)
    {
        GameObject portalProp = Instantiate(portalObject) as GameObject;
        //portalProp.AddComponent<Portal>();
        Portal p = portalProp.GetComponent<Portal>();
        // give each portal an ID
        p.PortalID = portalID;

        // give each portal a destination ID
        p.DestinationID = ((2 + p.PortalID) % NUM_PORTALS);
        if (debug) Debug.Log("Portal created with ID " + p.PortalID + " and DestinationId " + p.DestinationID);
        portals.Add(portalID, p);
        p.InitArtDisplay(roomController.GetRoomArt()[portalID]);
        //roomController.GetRoomArt()[portalID].SetParentUnityObject(p);
        return p;
    }

    public void DoTeleport(Player player, int portalID)
    {
        int destinationID = 0;
        float bump = 1.25f;
        if (!Locked || Locked) //HACK PROTOTYPE - disabled lockout to prevent players getting stuck
        {
            if (debug) Debug.Log("starting teleport form portal " + portalID);
            Vector3 destination = new Vector3(0, 20, 0);
            if (portalID < portals.Count)
            {
                for (int i = 0; i < portals.Count; i++)
                {
                    if (portals[i].PortalID == portals[portalID].DestinationID)
                    {
                        destinationID = portals[i].DestinationID;
                        destination = portals[i].gameObject.transform.position; // set destination to exit portal position
                    }
                }
            }
            else
            {
                bump = 1.5f;
                destinationID = sculptures[portalID - 4].DestinationID;
                destination = sculptures[destinationID].gameObject.transform.position; // set destination to exit portal position
            }
            // Bump player to just outside of the portal collision box based on the location of the portal relative to the center
            if (destination.x < 0)
            {
                destination.x += bump;
            }
            else
            {
                destination.x -= bump;
            }

            if (destination.z < 0)
            {
                destination.z += bump;
            }
            else
            {
                destination.z -= bump;
            }

            destination.y = 1f; // Fix exit height for player (player is 1.8 tall, portal is 5, center of portal is 2.5, center of player is 0.9. 2.5 - 0.9 = 1.6)

            player.transform.position = destination;

            /* FIXME Now tell the population controller that the player has moved 
             * by sending the portal (equiv to door) index to the population controller
             * 
             */

            ag.ChangeRoom(portalID, destinationID);

        }
        else
        {
            Locked = false;
        }

        SpawnPickups();

    }
}
