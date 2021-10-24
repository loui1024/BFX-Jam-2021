using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossmanMenu : MonoBehaviour
{
    public GameObject BossManPanel;
    public GameObject MadScientistPanel;


    private int m_CarPrice = 0;

   
    private int m_TankPrice = 0;


    //// TEXT
    public GameObject m_SelectCarTEXT;
    public GameObject m_SelectTankTEXT;
    public GameObject m_SelectBikeTEXT;
    public GameObject m_DefaultTEXT;
    public GameObject m_BuyCarTEXT;
    public GameObject m_BuyTankTEXT;
    public GameObject m_NotEnoughMoneyTEXT;
    public GameObject m_DefaultTextLabTEXT;

    //// BUTTONS

    public GameObject m_CarSelectBUTTON;
    public GameObject m_TankSelectBUTTON;
    public void GoToLab() {
        SetAllTextToFalse();
        m_DefaultTextLabTEXT.SetActive(true);
        BossManPanel.SetActive(false);
        MadScientistPanel.SetActive(true);
    }
    public void GoToNewsAgent() {
        SetAllTextToFalse();
        m_DefaultTEXT.SetActive(true);
        BossManPanel.SetActive(true);
        MadScientistPanel.SetActive(false);
    }
    public void SelectBicycle() {
        
        SetAllTextToFalse();
        m_SelectBikeTEXT.SetActive(true);
        Player.Instance.gameObject.GetComponent<SwitchVehicle>().SetBike();
        //Code to switch to bike
    }
    public void SelectTank() {
        SetAllTextToFalse();
        m_SelectTankTEXT.SetActive(true);
        Player.Instance.gameObject.GetComponent<SwitchVehicle>().SetTank();
    }
    public void SelectCar() {
        SetAllTextToFalse();
        m_SelectCarTEXT.SetActive(true);
        Player.Instance.gameObject.GetComponent<SwitchVehicle>().SetCar();
     
    }
    public void BuyTank() {
        if (Player.Instance.Money >= m_TankPrice) {
            Player.Instance.Money = Player.Instance.Money - m_TankPrice;

            SetAllTextToFalse();
            m_BuyTankTEXT.SetActive(true);
            m_TankSelectBUTTON.SetActive(true);

        }
        else {
            SetAllTextToFalse();
            m_NotEnoughMoneyTEXT.SetActive(true);
        }
    }

    public void BuyCar() {
        if (Player.Instance.Money >= m_CarPrice) {
            Player.Instance.Money = Player.Instance.Money - m_CarPrice;
           
            SetAllTextToFalse();
            m_BuyCarTEXT.SetActive(true);
            m_CarSelectBUTTON.SetActive(true);
           
        }
        else {
            SetAllTextToFalse();
            m_NotEnoughMoneyTEXT.SetActive(true);
        }
    }

    public void StartGame() {
        gameObject.SetActive(false);
    }

    private void SetAllTextToFalse() {
        m_SelectBikeTEXT.SetActive(false);
        m_SelectTankTEXT.SetActive(false);
        m_SelectCarTEXT.SetActive(false);
        m_DefaultTEXT.SetActive(false);

        m_BuyCarTEXT.SetActive(false);
        m_BuyTankTEXT.SetActive(false);
        m_NotEnoughMoneyTEXT.SetActive(false);
        m_DefaultTextLabTEXT.SetActive(false);
    }

}

