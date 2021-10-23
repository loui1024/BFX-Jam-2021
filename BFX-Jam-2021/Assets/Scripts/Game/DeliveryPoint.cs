using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[RequireComponent(typeof(Collider))]
public class DeliveryPoint : MonoBehaviour {

    /* EVENTS */
    public static        ItemDeliveredHandler ItemDelivered;
    public delegate void ItemDeliveredHandler(ItemDeliveredArgs _args);

    public class ItemDeliveredArgs : EventArgs {

        public ItemDeliveredArgs() {

        }
    }

    private void OnCollisionEnter(Collision collision) {

        if (collision.transform.tag == "Newspaper") {

            ItemDelivered?.Invoke(new ItemDeliveredArgs());

            Destroy(gameObject);
        }        
    }
}