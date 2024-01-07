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
using System;

class V_Content : UIWindow
{
    #region 脚本工具生成的代码
    private Button m_btn1;
    private Button m_btn2;
    private GameObject m_go1;
    private GameObject m_go2;
    protected override void ScriptGenerator()
    {
        m_go1 = FindChild("obj/m_go1").gameObject;
        m_go2 = FindChild("obj/m_go2").gameObject;
        m_btn1 = FindChildComponent<Button>("obj/m_btn1");
        m_btn2 = FindChildComponent<Button>("obj/m_btn2");
        m_btn1.onClick.AddListener(OnClick1Btn);
        m_btn2.onClick.AddListener(OnClick2Btn);
    }
    #endregion

    #region 事件
    private void OnClick1Btn()
    {
        m_go1.SetActive(true);
        m_go2.SetActive(false);
    }
    private void OnClick2Btn()
    {
        m_go1.SetActive(false);
        m_go2.SetActive(true);
    }
    #endregion

    protected override void OnFirstVisible()
    {
        base.OnFirstVisible();
        GameEvent.AddEventListener<int>(EntityEvent.BtnEvent, ObjChange);
    }

    public override void Close()
    {
        base.Close();
        GameEvent.RemoveEventListener<int>(EntityEvent.BtnEvent, ObjChange);
    }

    private void ObjChange(int obj)
    {
        switch (obj)
        {
            case 1:
                m_go1.SetActive(true);
                m_go2.SetActive(false);
                break;
            case 2:
                m_go1.SetActive(false);
                m_go2.SetActive(true);
                break;
        }
    }
}
