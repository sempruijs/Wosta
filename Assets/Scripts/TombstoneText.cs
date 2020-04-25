using TMPro;
using UnityEngine;

public class TombstoneText : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text DeadFraseText;

    private void Update()
    {
        if (GameManager.instance.state == GameManager.State.Dead)
        {
             var score = GameManager.instance.score;
             var highScore = PersistencyManager.instance.HighScore;
             scoreText.text = score.ToString();


             if (score == 0)
             {
                 DeadFraseText.text = "\"He wanted to see the sea but he died too young.\"";
             } else if (highScore == score)
             {
                 DeadFraseText.text = "\"He said: I wanna be the very best, Like no one ever was. And so he was.\"";
             } else if (score >= 40000 && score > highScore)
             {
                  DeadFraseText.text = "\"Press F to pay respects.\"";
             } else if (score > 15000 && score < 25000)
             {
                   DeadFraseText.text = "\"Look at me! he said, i am so big, I am going to win! And then he died.\"";                
             } else if (score > 27000 && score < 35000)
             {
                    DeadFraseText.text = "\"When it became night, he never saw daylight again.\"";                                
             } else if (score > 3000 && score < 10000)
             {
                 DeadFraseText.text = "\"He was a kind fish, knew when to eat and knew when to be friendly.\"";
             } else if (score < 1000 && score != 0)
             {
                   DeadFraseText.text = "\"He said: Mom, I am hungry. His mom said: Other fish are hungry to.\"";                
             }
             else
             {
                  DeadFraseText.text = "\"He had a special place in our hearts.\"";
             }
        }
    }
}
