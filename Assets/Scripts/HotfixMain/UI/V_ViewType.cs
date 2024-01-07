/*
 * created : 2023-03-01      author : holin
 * function description : 
 */
using TEngine;
using UnityEngine;
using UnityEngine.UI;

using TEngine.Runtime.UIModule;
using TEngine.Runtime.Entity;
using TEngine.Runtime;

class V_ViewType : UIWindow
{
    #region 脚本工具生成的代码
    private Button m_btn1;
    private Button m_btn2;
    protected override void ScriptGenerator()
    {
        m_btn1 = FindChildComponent<Button>("ViewType/Viewport/Content/m_btn1");
        m_btn2 = FindChildComponent<Button>("ViewType/Viewport/Content/m_btn2");
        m_btn1.onClick.AddListener(OnClick1Btn);
        m_btn2.onClick.AddListener(OnClick2Btn);
    }
    #endregion

    #region 事件
    private void OnClick1Btn()
    {
        GameEvent.Send<int>(EntityEvent.BtnEvent, 1);
    }
    private void OnClick2Btn()
    {
        GameEvent.Send<int>(EntityEvent.BtnEvent, 2);
    }
    #endregion



}
