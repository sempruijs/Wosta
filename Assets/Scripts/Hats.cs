using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Hats : MonoBehaviour
{
    
   public GameObject[] lockers; // index = hatID
   public GameObject[] cells;   // index = hatID
   public GameObject[] hats;    // (index / 2) = hatID
   public GameObject[] WosCoinHats; // index = hatID
   public List<int> ownedHatIDsNormal = new List<int>();
   public List<int> WosCoinHatIDsNormal = new List<int>();

   public int timesTouchedCrab = 0;
   public int timesTouchedWaterplant = 0;
   
   public static Hats instance;
   
   void Awake()
   {
       if (instance == null)
       {
           instance = this;
       }
       else if (instance != this)
       {
           Destroy(gameObject);
       }

       DontDestroyOnLoad(gameObject);
   }

   void Start()
   {
       Unlock(0);
       Unlock(1);
       Unlock(7);
       Unlock(8);
       Unlock(9);
   }

   private void Update()
   {
       openLockers();
       switch (GameManager.instance.level)
       {
           case 2:
               Unlock(2);
               break;
           case 3:
               Unlock(3);
               break;
           case 4:
               Unlock(5);
               break;
       }
   }

   public void openLockers()
   {
       foreach (int hatId in PersistencyManager.instance.NormalHatIds)
       {
           lockers[hatId].SetActive(false);
           cells[hatId].GetComponent<Button>().interactable = true;
       }
   }
   public void Unlock(int hatId)
   {
       var normalHatIds = PersistencyManager.instance.NormalHatIds;
       normalHatIds.Add(hatId);
       PersistencyManager.instance.NormalHatIds = normalHatIds;
   }

   public void SetActiveHat(int hatID)
   {
       noHat();
       hats[hatID * 2].SetActive(true);
       hats[hatID * 2 + 1].SetActive(true);                    
   }

   public void noHat()
   {
       foreach (GameObject hat in hats)
       {
           hat.SetActive(false);
       }

       foreach (GameObject hat in WosCoinHats)
       {
           hat.SetActive(false);
       }
   }
   
   // WosCoin hats
   public void SetActiveWosCoinHat(int hatID)
      {
          noHat(); 
          WosCoinHats[hatID * 2].SetActive(true);
          WosCoinHats[hatID * 2 + 1].SetActive(true);
      }
}
