using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchVehicle : MonoBehaviour
{

    public Transform m_Car;
    public Transform m_Bike;
    public Transform m_Tank;
    // Start is called before the first frame update


    public void SetBike() {
        DisableVehicles();
        m_Bike.gameObject.SetActive(true);
    }

    public void SetCar() {
        DisableVehicles();
        m_Car.gameObject.SetActive(true);
    }

    public void SetTank() {
        DisableVehicles();
        m_Tank.gameObject.SetActive(true);
    }

    public void DisableVehicles() {
        m_Car.gameObject.SetActive(false);
        m_Tank.gameObject.SetActive(false);
        m_Bike.gameObject.SetActive(false);
    }
}
