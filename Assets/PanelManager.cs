using System;
using System.Linq;
using UnityEngine;

namespace QFramework.Example
{
    [MonoSingletonPath("PanelManager")]
    public class PanelManager : MonoBehaviour,ISingleton
    {
        void ISingleton.OnSingletonInit()
        {
        }
        //单例
        private static PanelManager instance;
        public static PanelManager Instance
        {
            get
            {
                if (!instance)
                {
                    var uiRoot = UIRoot.Instance;
                    Debug.Log("currentUIRoot:" + uiRoot);
                    instance = MonoSingletonProperty<PanelManager>.Instance;
                }
                return instance;
            }
        }

        public void OpenPanel_Meeting()
        {
            SceneManager.Instance.UnloadExtraScene();
            SceneManager.Instance.LoadExtraScene("XD");

            UIKit.OpenPanel<UIMeetingPanel>(UILevel.Common, null, null, "UIPrefabs/UIMeetingPanel");
        }

        public void OpenPanel_ExtraInvitation()
        {
            //SceneManager.Instance.UnloadExtraScene();
            //SceneManager.Instance.LoadExtraScene("GC");

            UIKit.OpenPanel<UITipPanel_FirstMeeting>(UILevel.Common, null, null, "UIPrefabs/UITipPanel_FirstMeeting");
        }

        public void OpenPanel_SalesContract()
        {
            UIKit.OpenPanel<UISalesContractPanel>(UILevel.Common, null, null, "UIPrefabs/UISalesContractPanel");
        }

        public void OpenPanel_DialogueJournal()
        {
            UIKit.OpenPanel<UIDialogueJournalPanel>(UILevel.PopUI, null, null, "UIPrefabs/UIDialogueJournalPanel");
            //UIDialogueJournalPanel.SetActive(true);
        }

        public void OpenPanel_MakeTea()
        {
            UIKit.OpenPanel<UIMakeTeaPanel>(UILevel.Common, null, null, "UIPrefabs/UIMakeTeaPanel");
        }

        public void OpenPanel_FinalScore()
        {
            UIKit.OpenPanel<UIFinalScorePanel>(UILevel.Common, null, null, "UIPrefabs/UIFinalScorePanel");
        }
    }
}
