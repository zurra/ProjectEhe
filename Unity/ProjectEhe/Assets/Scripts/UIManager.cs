using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class UIManager : MonoBehaviour
    {
        public Text HealthText;
        public Text OppHealthText;
        public Text WinLose;
        public PlayerState PlayerState;
        public PlayerState EnemyPlayerState;
        public Text Input1;
        public Text Input2;
        public Text Input3;
        public Text Input4;
        public Text Input5;
        private int commandLine;

        public void Start()
        {
            commandLine = 1;
            Input1.enabled = false;
            Input2.enabled = false;
            Input3.enabled = false;
            Input4.enabled = false;
            Input5.enabled = false;
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

        public void ChangeCommandText(Enumerations.Action command)
        {
            switch (commandLine)
            {
                case 1:
                    Input1.enabled = true;
                    Input1.text = command.ToString();
                    commandLine++;
                    break;
                case 2:
                    Input2.enabled = true;
                    Input2.text = command.ToString();
                    commandLine++;
                    break;
                case 3:
                    Input3.enabled = true;
                    Input3.text = command.ToString();
                    commandLine++;
                    break;
                case 4:
                    Input4.enabled = true;
                    Input4.text = command.ToString();
                    commandLine++;
                    break;
                case 5:
                    Input5.enabled = true;
                    Input5.text = command.ToString();
                    commandLine = 1;
                    break;
            }        
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
