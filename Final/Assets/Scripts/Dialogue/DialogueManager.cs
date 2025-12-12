using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class DiaLogmanager : MonoBehaviour
{
  /// 对话内容文本，csv格式
    public TextAsset dialogDataFile;

    /// 角色名字文本
    public TMP_Text nameText;

    /// 对话内容文本
    public TMP_Text dialogText;

    /// 当前对话索引值
    public int dialogIndex;

    /// 对话文本按行分割
    public string[] dialogRows;

    public GameObject dialogueCanvas;

    /// 继续按钮
    public Button next;

    /// 选项按钮
    public GameObject optionButton;
    public GameObject optionImageButton;

    /// 选项按钮父节点
    public Transform buttonGroup;

    private PlayerData playerData;

    private void Awake()
    {
        
    }

    void Start()
    {
        playerData = DataManager.Instance.LoadCheckpoint();
        ReadText(dialogDataFile);
        ShowDiaLogRow();
    }

  // Update is called once per frame
    void Update()
    {
        
    }

  //更新文本信息
    public void UpdateText(string _name, string _text)
    {
        nameText.text = _name;
        dialogText.text = _text;
    }

    public void ReadText(TextAsset _textAsset)
    {
        dialogRows = _textAsset.text.Split('\n');//以换行来分割
        // foreach(var row in rows)
        //{
        // string[] cell = row.Split(',');
        // }
        Debug.Log("读取成果");
    }

    public void ShowDiaLogRow()
    {
        for(int i=0;i<dialogRows.Length;i++)
        {
            string[] cells = dialogRows[i].Split('\\');
            if (cells[0] == "content" && int.Parse(cells[1]) == dialogIndex)
            {
                UpdateText(cells[2], cells[3]);

                dialogIndex = int.Parse(cells[4]);
                next.gameObject.SetActive(true);
                break;
            }
            else if (cells[0]== "option" && int.Parse(cells[1]) == dialogIndex)
            {
                next.gameObject.SetActive(false);//隐藏原来的按钮
                if (cells[3].Contains("[image_"))
                {
                    GenerateImageOption(i);
                }
                else
                {
                    GenerateOption(i);
                }
            }
            else if (cells[0] == "end" && int.Parse(cells[1]) == dialogIndex)
            {
                dialogIndex = 1;
                dialogueCanvas.SetActive(false);
                Debug.Log("剧情结束");//这里结束
            }
            else if (cells[0] == "scene" && int.Parse(cells[1]) == dialogIndex)
            {
                SceneManager.LoadScene(cells[6].Trim(new char[] { ' ', '\t', '\n', '\r', '\"', '\'' }));
            }
        }
    }

    public void OnClickNext()
    {
        ShowDiaLogRow();
    }

    public void GenerateOption(int _index)//生成按钮
    {
        string[] cells = dialogRows[_index].Split('\\');
        if (cells[0] == "option")
        {
            GameObject button = Instantiate(optionButton, buttonGroup);

            //绑定按钮事件
            button.GetComponentInChildren<TMP_Text>().text = cells[3];
            button.GetComponent<Button>().onClick.AddListener(delegate {OnOptionClick(int.Parse(cells[4]));});
            GenerateOption(_index + 1);
        }
    }

    public void OnOptionClick(int _id)
    {
        dialogIndex = _id;
        ShowDiaLogRow();
        for(int i=0;i < buttonGroup.childCount; i++)
        {
            Destroy(buttonGroup.GetChild(i).gameObject);
        }
    }

    // 提取图片名称
    private string ExtractImageName(string text)
    {
        int start = text.IndexOf("[image_");
        if (start == -1) return "";

        int end = text.IndexOf("]", start);
        if (end == -1) return "";

        return text.Substring(start + 7, end - start - 7);
    }

    // 加载图片精灵
    private Sprite LoadImageSprite(string imageName)
    {
        try
        {
            string path = "OccultismSignals/" + imageName;
            Sprite sprite = Resources.Load<Sprite>(path);
            
            if (sprite == null)
            {
                Debug.LogWarning($"图片加载失败: {path}");
                return null;
            }
            
            return sprite;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载图片时出错: {e.Message}");
            return null;
        }
    }

    // 生成图片选项按钮
    public void GenerateImageOption(int _index)
    {
        string[] cells = dialogRows[_index].Split('\\');
        if (cells[0] == "option")
        {
            if (hasEndings(cells[5]))
            {
                GameObject button = Instantiate(optionImageButton, buttonGroup);

                // 提取图片名称
                string imageName = ExtractImageName(cells[3]);
                Sprite sprite = LoadImageSprite(imageName);

                // 设置按钮图片
                Image buttonImage = button.GetComponent<Image>();
                if (buttonImage != null && sprite != null)
                {
                    buttonImage.sprite = sprite;
                }

                // 设置按钮文本（如果有）
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    string text = cells[3].Replace($"[image_{imageName}]", "").Trim();
                    buttonText.text = text;
                }

                // 绑定按钮事件
                button.GetComponent<Button>().onClick.AddListener(
                    delegate { OnOptionClick(int.Parse(cells[4])); }
                );

                // 添加悬停提示
                ButtonHoverEffect hoverEffect = button.GetComponent<ButtonHoverEffect>();
                if (hoverEffect == null)
                {
                    hoverEffect = button.AddComponent<ButtonHoverEffect>();
                    hoverEffect.hoverScale = 1.1f;
                    hoverEffect.hoverColor = new Color(1f, 1f, 1f, 0.8f);
                }
            }

            // 继续生成下一个选项
            GenerateImageOption(_index + 1);
        }
    }

    private bool hasEndings(string endName)
    {
        switch (endName)
        {
            case "hasArrivedEmptyThrone":
                return playerData.hasArrivedEmptyThrone;
            case "hasDeadByTraps":
                return playerData.hasDeadByTraps;
            case "hasDeadbyIronVirgin":
                return playerData.hasDeadbyIronVirgin;
            case "hasInteractedWithTorch":
                return playerData.hasInteractedWithTorch;
            case "hasKilledBoss":
                return playerData.hasKilledBoss;
            case "hasFoundTheCandleHole":
                return playerData.hasFoundTheCandleHole;
            case "hasPassedCodeSpace":
                return playerData.hasPassedCodeSpace;
            case "hasKilledBossByTorch":
                return playerData.hasKilledBossByTorch;
            case "hasPassedHeaven":
                return playerData.hasPassedHeaven;
            default:
                return false;
        }
    }
}