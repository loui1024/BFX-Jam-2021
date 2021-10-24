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

        public DeliveryPoint m_Instigator { get; private set; }

        public ItemDeliveredArgs(DeliveryPoint _instigator) {
            m_Instigator = _instigator;
        }
    }

    private void OnTriggerEnter(Collider other) {

        if (other.transform.tag == "Newspaper") {

            ItemDelivered?.Invoke(new ItemDeliveredArgs(this));

            Destroy(gameObject);
        }        
    }
}
