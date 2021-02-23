using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinningTeamAnnouncement : MonoBehaviour
{
    private Text Text;
    
    // Start is called before the first frame update
    void Start()
    {
        Text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Text.text = CTFCharacterController.WinningTeam == Teams.Neutral
            ? ""
            : $"Team {CTFCharacterController.WinningTeam} wins!";
    }
}
