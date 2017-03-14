using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts
{
    public class UIManager : MonoBehaviour
    {
        public Text HealthText;
        public Text OppHealthText;
        public Text WinLose;
        public PlayerState PlayerState;
        public PlayerState EnemyPlayerState;

        public void Start()
        {
            WinLose.enabled = false;
        }

        //private void PlayerState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
            //if (e.PropertyName == "Health")
            //{
                //ChangeHealthText(((PlayerState)sender).Health);
                //return;
            //}
            //else if (e.PropertyName == "YouDead")
            //{
                //WinLose.text = "You Died!";
                //WinLose.enabled = true;
            //}
        //}

        //private void EnemyPlayerState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
            //if (e.PropertyName == "OppHealth")
            //{
                //ChangeOppText(((PlayerState)sender).Health);
                //return;
            //}
            //else if (e.PropertyName == "YouWin")
            //{
                //WinLose.text = "You Win!";
                //WinLose.enabled = true;
            //}
        //}

        public void Win()
        {
            WinLose.text = "You Win!";
            WinLose.enabled = true;
        }

        public void Lose()
        {
            WinLose.text = "You Died!";
            WinLose.enabled = true;
        }

        public void ChangeHealthText(int temp)
        {
            HealthText.text = temp.ToString();
        }

        public void ChangeOppText(int temp)
        {
            OppHealthText.text = temp.ToString();
        }

        //public void SetPlayerState(PlayerState playerstate)
        //{
            //PlayerState = playerstate;
            //PlayerState.PropertyChanged += PlayerState_PropertyChanged;
            //ChangeHealthText(10);
        //}

        //public void SetEnemyPlayerState(PlayerState playerstate)
        //{
            //EnemyPlayerState = playerstate;
            //EnemyPlayerState.PropertyChanged += EnemyPlayerState_PropertyChanged;
            //ChangeOppText(10);
        //}
    }
}
