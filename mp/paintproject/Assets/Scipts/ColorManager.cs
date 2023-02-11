using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class ColorManager : MonoBehaviour
{
    public delegate void OnChooseColorDelegate(int index);
    public event OnChooseColorDelegate OnChooseColor;

    const string strLayerImage = "Image";
    int nLayerImage;

    public Texture2D[] baseTexture;
    public Texture2D[] changeTexture, finalTexture;

    public List<GridPart> colorGrid = new List<GridPart>();
    public float maxDifference;

    public Image showImage;
    public GameObject numberPrefab;

    public float colorSpeed;
    public int maxPositionDifference;

    public GameObject colorButtonPrefab;

    public Transform colorParent;

    public CalculateManager calculator;

    public static ColorManager Instance;

    int choosenIndex;

    List<NumberPrefabController> showTexts = new List<NumberPrefabController>();

    public float maxClickDistance;

    public int level;

    public List<GameObject> buttons = new List<GameObject>();

    int colorRoutineCount, lastIndex, lastColumn;

    void Awake()
    {
        if(Instance == null) Instance = this;
    }
    void Start()
    {

        nLayerImage = LayerMask.NameToLayer(strLayerImage);

        OnPrepareNewGame();
        
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            NumberPrefabController controller = GetClosestNumber( GetMousePosition(GetEventSystemRaycastResults()) );
            if(controller != null && controller.index == choosenIndex)
            {
                StartCoroutine(ChangeColorRoutine(controller.index));
                showTexts.Remove(controller);
                Destroy(controller.gameObject);

                
            }
        }
    }

    IEnumerator ChangeColorRoutine(int index)
    {
        float end = Time.time + 3f;
        colorRoutineCount++;
        while(true)
        {
            List<Pixel> temp = colorGrid[index].pixels;

            Color col = colorGrid[index].color;

            for(int i=0; i<temp.Count; i++)
            {
                col = temp[i].color;
                changeTexture[GetLevelIndex()].SetPixel(temp[i].row, temp[i].column, Color.Lerp(changeTexture[GetLevelIndex()].GetPixel(temp[i].row, temp[i].column), col, Time.deltaTime * colorSpeed)); 
            }

            changeTexture[GetLevelIndex()].Apply();

            if(Time.time > end) break;

            yield return null;
        }

        colorRoutineCount--;
        if(colorRoutineCount == 0)
        {
            if(showTexts.Count == 0)
            {
                level++;
                OnPrepareNewGame();
            }
        }
    }

    void OnPrepareNewGame()
    {
        PrepareParts(finalTexture[GetLevelIndex()]);
        LoadTexture();

        ClearLevel();

        Texture2D texture = changeTexture[GetLevelIndex()];

        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

        showImage.sprite = sprite;

        for(int i=0; i<colorGrid.Count; i++)
        {
            colorGrid[i].CalculatePosition(calculator.GetMiddlePoint(colorGrid[i].pixels, maxPositionDifference));
        }

        for(int i=0; i<colorGrid.Count; i++)
        {
            SpawnNumber(baseTexture[GetLevelIndex()], i);
        }

        SpawnColorButtons();

        choosenIndex = -1;
    }

    void ClearLevel()
    {
        int count = showTexts.Count;
        for(int i=0; i<count; i++)
        {
            Destroy(showTexts[i].gameObject);
        }

        count = buttons.Count;
        for(int i=0; i<count; i++)
        {
            Destroy(buttons[i].gameObject);
        }

        showTexts = new List<NumberPrefabController>();
        buttons = new List<GameObject>();
        
    }

    int GetLevelIndex()
    {
        if(level >= baseTexture.Length) level = Random.Range(0, baseTexture.Length);
        int num = level;
        

        return num;
    }

    NumberPrefabController GetClosestNumber(Vector3 clickPos)
    {
        float dist = maxClickDistance;

        NumberPrefabController result = null;

        for(int i=0; i<showTexts.Count; i++)
        {
            float temp = Vector3.Distance(showTexts[i].transform.position, clickPos);
            if( temp < dist )
            {
                dist = temp;
                result = showTexts[i];
            }
        }

        return result;
    }

    public void OnChooseColorButton(int index)
    {
        choosenIndex = index;
        ChooseColor(choosenIndex);
    }

    void OnClickColor()
    {
        
    }

    void SpawnColorButtons()
    {
        for(int i=0; i<colorGrid.Count; i++)
        {
            GameObject but = Instantiate(colorButtonPrefab, colorParent);

            ColorButtonController controller = but.GetComponent<ColorButtonController>();

            controller.Init(i, colorGrid[i].color);

            buttons.Add(but);
        }
    }

    void LoadTexture()
    {
        changeTexture[GetLevelIndex()].SetPixels(baseTexture[GetLevelIndex()].GetPixels());
        changeTexture[GetLevelIndex()].Apply();
    }

    void PrepareParts(Texture2D tex)
    {
        int width = tex.width;
        int height = tex.height;

        colorGrid = new List<GridPart>();

        for(int i=0; i<width; i++)
        {
            for(int k=0; k<height; k++)
            {

                Color color = tex.GetPixel(i, k);

                if(!AlreadyChecked(color))
                {
                    GridPart part = new GridPart();
                    part.pixels = new List<Pixel>();
                    part.AddPixel(i, k, color);
                    part.color = color;

                    colorGrid.Add(part);
                }
                else
                {
                    colorGrid[lastIndex].AddPixel(i, k, color);
                }
            
            }
        }
    }

    bool AlreadyChecked(Color color)
    {
        
        //if(SameColors(color, Color.black) ) return true;
        //if(SameColors(color, Color.white) ) return true;
        for(int i=0; i<colorGrid.Count; i++)
        {
            if(SameColors(colorGrid[i].color, color))
            {
                lastIndex = i;
                return true;
            }
        }

        return false;
    }

    bool SameColors(Color newColor, Color checkColor)
    {
        float diff = newColor.r - checkColor.r;

        if(Mathf.Abs(diff) < maxDifference)
        {
            return true;
        }

        diff = newColor.g - checkColor.g;

        if(Mathf.Abs(diff) < maxDifference)
        {
            return true;
        }

        diff = newColor.b - checkColor.b;

        if(Mathf.Abs(diff) < maxDifference)
        {
            return true;
        }

        else return false;

    }

    public void SpawnNumber(Texture2D tex, int gridIndex)
    {
        float width = tex.width;
        float height = tex.height;

        float y2 = showImage.GetComponent<RectTransform>().rect.width;
        float x2 = showImage.GetComponent<RectTransform>().rect.height;

        float scaleFactorX = x2 / width;
        float scaleFactorY = y2 / height;

        float x = colorGrid[gridIndex].middleRow;
        float y = colorGrid[gridIndex].middleColumn;

        Debug.Log(scaleFactorX+" "+scaleFactorY+" "+width+" "+x2);

        Vector3 vec = new Vector3(x *scaleFactorX - x2 / 2f, y *scaleFactorY - y2 / 2f, 0);

        

        Transform temp = Instantiate(numberPrefab, showImage.transform).transform;

        NumberPrefabController controller = temp.GetComponent<NumberPrefabController>();

        controller.row = (int) x;
        controller.column = (int) y;
        controller.index = gridIndex;

        temp.GetComponent<TMP_Text>().text = gridIndex.ToString();

        temp.localPosition = vec;

        showTexts.Add(controller);
    }


    Vector3 GetMousePosition(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == nLayerImage)
                {
                    return curRaysastResult.worldPosition;
                    //return curRaysastResult.gameObject.GetComponent<NumberPrefabController>();
                }
        }
        return Vector3.zero;
    }

    
    private List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    public void ChooseColor(int index)
    {
        OnChooseColor?.Invoke(index);
    }

    
}


