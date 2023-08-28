using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunker : MonoBehaviour
{
    private const int MaxNumberChunks = 9;  //For Each Chunk Around the Center Chunk

    private GameObject CurrentChunk;
    private float ChunkXSize;
    private float ChunkZSize;
    public float perlinScale = 1;
    private Vector2 randPerlin;
    private Vector3 InitPos;

    [Header("Tiles")]
    public List<GameObject> WorldChunks = new List<GameObject>();

    [Header("Tile Materials")]
    public List<Material> WorldMaterials = new List<Material>();

    [Header("Map Generation Values")]
    public Vector3 TileChangeDistance = new Vector3(100.0f, 0.0f, 100.0f);

    // Start is called before the first frame update
    void Start()
    {
        //Set random perlin offset
        randPerlin.x = Random.Range(0f, 99999f);
        randPerlin.y = Random.Range(0f, 99999f);

        //Set Center Chunk
        CurrentChunk = WorldChunks[0];

        //Set Scale Diff (If placed flush to center the Top Right Corner will give Positive Size Values)
        ChunkXSize = WorldChunks[3].transform.position.x;
        ChunkZSize = WorldChunks[3].transform.position.z;

        //Set the Initial Position
        InitPos = this.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        WorldReArranger(other);
    }

    void WorldReArranger(Collider _other)
    {
        if (CurrentChunk.name != _other.transform.parent.name)
        {
            //Update Current Chunk
            for (int i = 0; i < MaxNumberChunks; i++)
            {
                if (WorldChunks[i] == _other.transform.parent.gameObject)
                {
                    //Position in Array (To Match the Chunk Locations)
                    WorldChunks[0] = _other.transform.parent.gameObject;
                    WorldChunks[i] = CurrentChunk;

                    Transform TempPos = WorldChunks[i].transform;
                    WorldChunks[i].transform.position = WorldChunks[0].transform.position;
                    WorldChunks[0].transform.position = TempPos.position;

                    //Set to 0 Directional Material
                    Material new_material = Instantiate(WorldMaterials[Random.Range(0, WorldMaterials.Count)]);

                    WorldChunks[0].transform.GetChild(0).GetComponent<MeshRenderer>().material = new_material;

                    CurrentChunk = WorldChunks[0];

                    break;
                }
            }

            
            for (int i = 0; i < MaxNumberChunks; i++)
            {
                ChunkPositioner(i);

                TileChanger(WorldChunks[i]);

                
            }
                
            //Set up another for loop to add blended materials
            for (int i = 0; i < MaxNumberChunks; i++)
            {
                Debug.Log("i: " + i + "\nMaterial: " + GetMaterialIndex(WorldChunks[i].transform.GetChild(0).GetComponent<MeshRenderer>().material));

                TileBlender(i, GetMaterialIndex(WorldChunks[i].transform.GetChild(0).GetComponent<MeshRenderer>().material));
            }
        }
    }

    int WorldTilePicker(Vector3 _tilepos)
    {
        int MaterialIndex = 0;

        ////Beyond Material 2
        //if (Vector3.Distance(InitPos, _tilepos) > Vector3.Distance(InitPos, (InitPos + (2 * TileChangeDistance))))
        //{
        //    MaterialIndex = 1;
        //}

        ////Material 2
        //if (Vector3.Distance(InitPos, _tilepos) < Vector3.Distance(InitPos, (InitPos + (2 * TileChangeDistance))) && Vector3.Distance(InitPos, _tilepos) > Vector3.Distance(InitPos, (InitPos + TileChangeDistance)))
        //{
        //    MaterialIndex = 1;
        //}

        ////Material 1
        //if (Vector3.Distance(InitPos, _tilepos) < Vector3.Distance(InitPos, (InitPos + TileChangeDistance)))
        //{
        //    MaterialIndex = 0;
        //}

        //Get the player pos as arguments for perlin noise parameters. Divide them by chunk size so they go from 0 - 1
        //Vector3 playerPos = gameObject.transform.parent.transform.position;
        float XCoord = _tilepos.x / ChunkXSize * perlinScale + randPerlin.x;
        float ZCoord = _tilepos.z / ChunkZSize * perlinScale + randPerlin.y;
        float sample = Mathf.PerlinNoise(XCoord, ZCoord);
        //Debug.Log("Perlin result: " + sample);

        //Determine material index based on whether the perlin noise reached past 3 or 6
        if (sample >= 0.5f)
        {
            MaterialIndex = 1;
        }
        //else if (sample > 0.3f)
        //{
        //    MaterialIndex = 1;
        //}

        return MaterialIndex;
    }

    void TileChanger(GameObject _tile)
    {
        int tilePick = WorldTilePicker(_tile.transform.position);
        Material new_material = Instantiate(WorldMaterials[tilePick]);

        //Setting the material index to the blended material should the previous one not match with current material
        //if (tilePick != _prevMaterialIndex && (_prevMaterialIndex != -1 && _prevMaterialIndex != 2))
        //{
        //    new_material = WorldMaterials[2];
        //}

        _tile.transform.GetChild(0).GetComponent<MeshRenderer>().material = new_material;
    }

    //Rotate blended tiles
    void TileBlender(int _tilePos, int _tileMaterial)
    {
        //Dictionary<string, bool> diffMaterials = new Dictionary<string, bool>();
        List<string> directions = new List<string>();
        int otherMaterialIndex = 0;

        //Check surrounding tiles to see which tiles need blending
        for (int i = _tilePos - 3; i <= (_tilePos + 3); i += 2)
        {
            //Skip this iteration if the material index is blended texture or material index is invalid
            if ((i < 0 || i >= MaxNumberChunks) || 
                GetMaterialIndex(WorldChunks[i].transform.GetChild(0).GetComponent<MeshRenderer>().material) == -1 ||
                GetMaterialIndex(WorldChunks[i].transform.GetChild(0).GetComponent<MeshRenderer>().material) == 2) continue;

            string tileDirection = "";
            bool isDifferent = (GetMaterialIndex(WorldChunks[i].transform.GetChild(0).GetComponent<MeshRenderer>().material) != _tileMaterial) ? true : false;

            //If the material is the same then move on to the next tile
            if (!isDifferent) continue;

            //Check the row limits
            int rowNum = _tilePos / 3;

            //Top
            if (i == (_tilePos - 3))
            {
                tileDirection = "top";
                
            }

            //Left
            if (i == (_tilePos - 1) && (i / 3) == rowNum)
            {
                tileDirection = "left";
            }

            //Right
            if (i == (_tilePos + 1) && (i / 3) == rowNum)
            {
                tileDirection = "right";
            }

            //Bottom
            if (i == (_tilePos + 3))
            {
                tileDirection = "bottom";
            }

            //Add to list
            if (tileDirection != "")
            {
                //diffMaterials.Add(tileDirection, isDifferent);
                directions.Add(tileDirection);
                otherMaterialIndex = i;
            }

            //Debug.Log("I: " + i + "\nJ: " + j);
        }

        //Check if there are any directions for blended tiles
        if (directions.Count == 0) return;

        //Check if there are any valid adjacent directions
        string initDirection = directions[0];
        string adjacent = (directions.Count >= 2) ? directions[1] : "";
        int rotation = 0;

        /*
         In theory, the initDirection string should never have "bottom" and the adjacent string
         should never have "top" due to how the previous for loop checks for valid directions from the top 
         down and because I am only accessing the first two elements in the directions list but this is just 
         a precaution for tiles that could be in between two different biomes.
         
         e.g: if the tile pos is on the first row then "left" or "right" will be the first value on 
         initDirection and "right" or "bottom" will be the first value in the adjacent value respectively.
         
         If the tile pos is on the second row then "top" will always be the first value on initDirection
         and "left" or "right" will be in the adjacent value

         Lastly, if the tile pos is on the third row then it should be the same as the second row.

         Otherwise, if the initDirection and adjacent are at opposite directions ("top" and "bottom"
         or "left" and "right") that means they're two different biomes and should be ignored

         - Matthew
         */
        switch (initDirection)
        {
            case "left":
                rotation = -90;
                break;
            case "right":
                rotation = 90;
                break;
            case "bottom":
                rotation = -180;
                break;
            default:
                break;
        }

        bool isOpposite = false;

        if (adjacent != "")
        {
            int rotateAdjustment = 0;
            switch (initDirection)
            {
                case "top":
                //Again, initDirection will probably never be set to "bottom" but this is just a precaution
                case "bottom":
                    if (adjacent != "left" && adjacent != "right")
                    {
                        isOpposite = true;
                        break;
                    }

                    rotateAdjustment = (adjacent == "left") ? -45 : 45;

                    if (initDirection == "bottom") rotation = -180 - rotateAdjustment;
                    else rotation += rotateAdjustment;
                    break;
                case "left":
                case "right":
                    //Adjacent will probably never be set to "top" but this is just a precaution
                    if (adjacent != "top" && adjacent != "bottom")
                    {
                        isOpposite = true;
                        break;
                    }

                    rotateAdjustment = (adjacent == "top") ? -45 : 45;

                    if (initDirection == "left") rotation = -90 - rotateAdjustment;
                    else rotation += rotateAdjustment;
                    break;
            }
        }

        //Do not blend if the two different tiles are at opposite directions
        if (isOpposite) return;

        //Get the material shader graph rotation value
        Material blendMaterial = Instantiate(WorldMaterials[2]);
        //Set rotation onto the tile
        blendMaterial.SetFloat("Rotation", rotation);
        
        //Set Texture onto tile
        blendMaterial.SetTexture("Blend A", WorldMaterials[_tileMaterial].mainTexture);
        //Find the other texture through initdirection
        Texture otherTex = WorldChunks[otherMaterialIndex].transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture;
        blendMaterial.SetTexture("Blend B", otherTex);
        WorldChunks[_tilePos].transform.GetChild(0).GetComponent<MeshRenderer>().material = blendMaterial;
    }

    void TileRotator(int _i, float _Xpos, float _Zpos)
    {
        float oldperlin = Mathf.PerlinNoise(WorldChunks[_i].transform.position.x, WorldChunks[_i].transform.position.z);

        if (_i == 1)
        {
            //Top Left
            if (Mathf.Abs(oldperlin - Mathf.PerlinNoise(_Xpos, _Zpos)) <= 0.1f)
            {
                Material new_material = Instantiate(WorldMaterials[0]);

                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material = new_material;
                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("Rotation", 135.0f);
            }
        }

        //Top
        if (_i == 2)
        {
            if (Mathf.Abs(oldperlin - Mathf.PerlinNoise(_Xpos, _Zpos)) <= 0.1f)
            {
                Material new_material = Instantiate(WorldMaterials[0]);

                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material = new_material;
                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("Rotation", 90.0f);
            }
        }

        if (_i == 3)
        {
            //Top Right
            if (Mathf.Abs(oldperlin - Mathf.PerlinNoise(_Xpos, _Zpos)) <= 0.1f)
            {
                Material new_material = Instantiate(WorldMaterials[0]);

                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material = new_material;
                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("Rotation", 45.0f);
            }
        }

        if (_i == 4)
        {
            //Left
            if (Mathf.Abs(oldperlin - Mathf.PerlinNoise(_Xpos, _Zpos)) <= 0.1f)
            {
                Material new_material = Instantiate(WorldMaterials[0]);

                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material = new_material;
                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("Rotation", 180.0f);
            }
        }

        if (_i == 5)
        {
            //Right
            if (Mathf.Abs(oldperlin - Mathf.PerlinNoise(_Xpos, _Zpos)) <= 0.1f)
            {
                Material new_material = Instantiate(WorldMaterials[0]);

                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material = new_material;
                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("Rotation", 360.0f);
            }
        }

        if (_i == 6)
        {
            //Bottom Left
            if (Mathf.Abs(oldperlin - Mathf.PerlinNoise(_Xpos, _Zpos)) <= 0.1f)
            {
                Material new_material = Instantiate(WorldMaterials[0]);

                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material = new_material;
                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("Rotation", -135.0f);
            }
        }

        //Bottom
        if (_i == 7)
        {
            if (Mathf.Abs(oldperlin - Mathf.PerlinNoise(_Xpos, _Zpos)) <= 0.1f)
            {
                Material new_material = Instantiate(WorldMaterials[0]);

                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material = new_material;
                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("Rotation", -90.0f);
            }
        }

        if (_i == 8)
        {
            //Bottom Right
            if (Mathf.Abs(oldperlin - Mathf.PerlinNoise(_Xpos, _Zpos)) <= 0.1f)
            {
                Material new_material = Instantiate(WorldMaterials[0]);

                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material = new_material;
                WorldChunks[_i].transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("Rotation", -45.0f);
            }
        }
    }

    void ChunkPositioner(int _i)
    {
        if (_i == 1)
        {
            //Top Left
            float Xpos = WorldChunks[0].transform.position.x - (WorldChunks[_i].transform.localScale.x * ChunkXSize);
            float Zpos = WorldChunks[0].transform.position.z + (WorldChunks[_i].transform.localScale.z * ChunkZSize);

            WorldChunks[_i].transform.position = new Vector3(Xpos, 0, Zpos);
        }

        if (_i == 2)
        {
            //Top
            float Xpos = WorldChunks[0].transform.position.x;
            float Zpos = WorldChunks[0].transform.position.z + (WorldChunks[_i].transform.localScale.z * ChunkZSize);

            WorldChunks[_i].transform.position = new Vector3(Xpos, 0, Zpos);
        }

        if (_i == 3)
        {
            //Top Right
            float Xpos = WorldChunks[0].transform.position.x + (WorldChunks[_i].transform.localScale.x * ChunkXSize);
            float Zpos = WorldChunks[0].transform.position.z + (WorldChunks[_i].transform.localScale.z * ChunkZSize);

            WorldChunks[_i].transform.position = new Vector3(Xpos, 0, Zpos);
        }

        if (_i == 4)
        {
            //Left
            float Xpos = WorldChunks[0].transform.position.x - (WorldChunks[_i].transform.localScale.x * ChunkXSize);
            float Zpos = WorldChunks[0].transform.position.z;

            WorldChunks[_i].transform.position = new Vector3(Xpos, 0, Zpos);
        }

        if (_i == 5)
        {
            //Right
            float Xpos = WorldChunks[0].transform.position.x + (WorldChunks[_i].transform.localScale.x * ChunkXSize);
            float Zpos = WorldChunks[0].transform.position.z;

            WorldChunks[_i].transform.position = new Vector3(Xpos, 0, Zpos);
        }

        if (_i == 6)
        {
            //Bottom Left
            float Xpos = WorldChunks[0].transform.position.x - (WorldChunks[_i].transform.localScale.x * ChunkXSize);
            float Zpos = WorldChunks[0].transform.position.z - (WorldChunks[_i].transform.localScale.z * ChunkZSize);

            WorldChunks[_i].transform.position = new Vector3(Xpos, 0, Zpos);
        }

        if (_i == 7)
        {
            //Bottom
            float Xpos = WorldChunks[0].transform.position.x;
            float Zpos = WorldChunks[0].transform.position.z - (WorldChunks[_i].transform.localScale.z * ChunkZSize);

            WorldChunks[_i].transform.position = new Vector3(Xpos, 0, Zpos);
        }

        if (_i == 8)
        {
            //Bottom Right
            float Xpos = WorldChunks[0].transform.position.x + (WorldChunks[_i].transform.localScale.x * ChunkXSize);
            float Zpos = WorldChunks[0].transform.position.z - (WorldChunks[_i].transform.localScale.z * ChunkZSize);

            WorldChunks[_i].transform.position = new Vector3(Xpos, 0, Zpos);
        }
    }

    int GetMaterialIndex(Material _tileMaterial)
    {
        for (int i = 0; i < WorldMaterials.Count; i++)
        {
            if (WorldMaterials[i].mainTexture == _tileMaterial.mainTexture) return i;
        }

        Debug.LogWarning("Could not find tile material");
        return -1;
    }
}
