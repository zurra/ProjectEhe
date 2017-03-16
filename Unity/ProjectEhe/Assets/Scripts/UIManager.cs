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

        private Color _originalColor;

        public void Start()
        {
            commandLine = 1;
            Input1.enabled = false;
            Input2.enabled = false;
            Input3.enabled = false;
            Input4.enabled = false;
            Input5.enabled = false;
            WinLose.enabled = false;
            _originalColor = Input1.transform.parent.GetComponent<Image>().color;
        }

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

        public void ClearCommands()
        {
            Input1.text = string.Empty;
            Input2.text = string.Empty;
            Input3.text = string.Empty;
            Input4.text = string.Empty;
            Input5.text = string.Empty;
        }

        public void DisplayActiveCommand(int i)
        {
            if (i == 0)
            {
                Input1.transform.parent.GetComponent<Image>().color = Color.white;

                SetOriginalTextParentBackgroundColor(Input2);
                SetOriginalTextParentBackgroundColor(Input3);
                SetOriginalTextParentBackgroundColor(Input4);
                SetOriginalTextParentBackgroundColor(Input5);
            }
            else if (i == 1)
            {
                Input2.transform.parent.GetComponent<Image>().color = Color.white;
            }
            else if (i == 2)
            {
                Input3.transform.parent.GetComponent<Image>().color = Color.white;
            }
            else if (i == 3)
            {
                Input4.transform.parent.GetComponent<Image>().color = Color.white;
            }
            else if (i == 4)
            {
                Input5.transform.parent.GetComponent<Image>().color = Color.white;
            }
        }

        void SetOriginalTextParentBackgroundColor(Text txt)
        {
            txt.transform.parent.GetComponent<Image>().color = _originalColor;
        }
    }
}
