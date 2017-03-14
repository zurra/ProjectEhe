using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts
{
    public class UIManager : MonoBehaviour
    {
        public Text HealthText;
        public PlayerState PlayerState;

        public void Start()
        {
        }

        private void PlayerState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.Log("Trying to lose health");
            if (e.PropertyName == "Health")
            {
                ChangeText(PlayerState.Health);
                return;
            }
        }

        public void ChangeText(int temp)
        {
            HealthText.text = temp.ToString();
        }
        
        public void SetPlayerState(PlayerState playerstate)
        {
            PlayerState = playerstate;
            PlayerState.PropertyChanged += PlayerState_PropertyChanged;
            ChangeText(10);
        }        
    }
}
