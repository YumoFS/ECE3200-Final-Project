using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrambledTexts : MonoBehaviour
{
    private const int TEXTCOLUMN = 22;
    private const int TEXTRAW = 6;
    [SerializeField] private int randomAbleNum = 7;
    [SerializeField] private GameObject interactiveTextPrefab;
    private float counter = 0;
    private float fourCharactersWidth = 0;
    private float fourCharactersHeight = 0;
    private string password = "123";


    private void Awake()
    {
        RebuildMatrix();
    }
    private void Update()
    {
        counter += Time.deltaTime;
        if (counter >= 7f)
        {
            counter = 0;
            RebuildMatrix();
        }
    }




    private void RebuildMatrix()
    {
        InteractiveText[] existedInteractiveTexts = GetComponentsInChildren<InteractiveText>();
        int existedLen = existedInteractiveTexts.Length;
        for (int i = 0; i < existedLen; i ++)
        {
            existedInteractiveTexts[i].DestroySelf();
        }
        int[][] ablePoints = GetRandomAblePoints();
        for (int i = 0; i < TEXTCOLUMN; i ++)
        {
            for (int j = 0; j < TEXTRAW; j ++)
            {
                bool isAble = false;
                for (int k = 0; k < ablePoints.Length; k ++)
                {
                    if(i == ablePoints[k][0] && j == ablePoints[k][1])
                        isAble = true;
                }
                if (isAble)
                {
                    GenerateAbleAt(i, j);
                }
                else
                {
                    GenerateCodeAt(i, j, Generate4RandomCodeExceptABLE());
                }
            }
        }
    }

    private InteractiveText GenerateCodeAt(int col, int raw, string code)
    {
        string first4Code = code[..4]; 
        GameObject generateText = Instantiate(interactiveTextPrefab);
        generateText.transform.SetParent(transform);
        InteractiveText generateTextIT = generateText.GetComponent<InteractiveText>();
        generateTextIT.SetTextContent(first4Code, InteractiveText.DEFAULT_FONTSIZE);
        if(fourCharactersHeight == 0 || fourCharactersWidth == 0)
        {
            fourCharactersWidth = generateTextIT.GetCollider().size.x;
            fourCharactersHeight = generateTextIT.GetCollider().size.y;
        }
        Vector3 generateTextPosition = new Vector3(col*fourCharactersWidth, -raw*fourCharactersHeight, 0);
        generateText.transform.SetLocalPositionAndRotation(generateTextPosition, new Quaternion());
        return generateTextIT;
    }
    private InteractiveText GenerateAbleAt(int col, int raw)
    {
        InteractiveText generateTextIT = GenerateCodeAt(col, raw, "able");
        generateTextIT.password = password;
        return generateTextIT;
    }
    private string Generate4RandomCodeExceptABLE()
    {
        string randomCode = "";
        for (int i = 0; i < 4; i ++)
        {
            char c = (char)UnityEngine.Random.Range(33, 127);
            randomCode += c;
        }
        bool isFirstCharacterA = randomCode[0] == 'A' || randomCode[0] == 'a';
        bool isSecondCharacterB = randomCode[0] == 'B' || randomCode[0] == 'b';
        bool isThirdCharacteL = randomCode[0] == 'L' || randomCode[0] == 'l';
        bool isFourthCharacterE = randomCode[0] == 'E' || randomCode[0] == 'e';
        if(isFirstCharacterA && isSecondCharacterB && isThirdCharacteL && isFourthCharacterE)
        {
            return Generate4RandomCodeExceptABLE();  // Regenerate
        }
        return randomCode;
    }
    private int[][] GetRandomAblePoints()
    {
        if (randomAbleNum > TEXTCOLUMN*TEXTRAW/2)
        {
            randomAbleNum = TEXTCOLUMN*TEXTRAW/2;
        }
        int[][] res = new int[randomAbleNum][];
        for (int i = 0; i < randomAbleNum; i ++)
        {
            res[i] = new int[2];
        } 
        int cnt = 0;
        while (cnt < randomAbleNum)
        {
            int randX = UnityEngine.Random.Range(0, TEXTCOLUMN), randY = UnityEngine.Random.Range(0, TEXTRAW);
            bool isRepeated = false;
            for (int i = 0; i < cnt; i ++)
            {
                if (res[i][0] == randX && res[i][1] == randY) isRepeated = true;
            }
            if (!isRepeated)
            {
                res[cnt][0] = randX; res[cnt][1] = randY;
                cnt ++;
            }
        }
        return res;
    }
}
