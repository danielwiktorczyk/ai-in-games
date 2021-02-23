using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HasFlagText : MonoBehaviour
{
    public Teams Team;
    private Text Text;

    // Start is called before the first frame update
    void Start()
    {
        Text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (Team)
        {
            case Teams.Red:
                Text.text = CTFCharacterController.RedFlagSeekerActiveWithFlag && CTFCharacterController.RedFlagSeekerActive
                    ? $"{Team} has the flag!"
                    : "";
                break;
            case Teams.Blue:
                Text.text = CTFCharacterController.BlueFlagSeekerActiveWithFlag && CTFCharacterController.BlueFlagSeekerActive
                    ? $"{Team} has the flag!"
                    : "";
                break;
            case Teams.Green:
                Text.text = CTFCharacterController.GreenFlagSeekerActiveWithFlag && CTFCharacterController.GreenFlagSeekerActive
                    ? $"{Team} has the flag!"
                    : "";
                break;
            case Teams.Neutral:
                break;
        }
    }
}
