using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    /// 继续按钮
    public Button next;

    /// 选项按钮
    public GameObject optionButton;

    /// 选项按钮父节点
    public Transform buttonGroup;

    private void Awake()
    {
        
    }

    void Start()
    {
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
            GenerateOption(i);
        }
        else if (cells[0] == "end" && int.Parse(cells[i]) == dialogIndex)
        {
            Debug.Log("剧情结束");//这里结束
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
}