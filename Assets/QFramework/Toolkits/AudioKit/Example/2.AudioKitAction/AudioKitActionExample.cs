using UnityEngine;

namespace QFramework.Example
{
    public class AudioKitActionExample : MonoBehaviour
    {
        private void Start()
        {
            PlaySoundAction
                .Allocate("resources://button_clicked", () =>
                {
                    Debug.Log("button_clicked finish");
                })
                .Start(this);

            var heroClip = Resources.Load<AudioClip>("hero_hurt");

            ActionKit.Sequence()
                .Delay(1.0f)
                .PlaySound("resources://button_clicked")
                .Delay(1.0f)
                .PlaySound(heroClip)
                .Start(this);
        }
    }
}