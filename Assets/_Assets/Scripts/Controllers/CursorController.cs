using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    /// <summary>
    /// Returns a list of hexagons nearby the cursor,
    /// ordered by their y position value.
    /// </summary>
    /// <returns>List of hexagon controllers</returns>
    public List<HexagonController> GetNearbyHexagons()
    {
        var colliders = Physics.OverlapSphere(transform.position, 0.25f);
        var controllers = new List<HexagonController>();

        foreach (var coll in colliders)
        {
            var controller = coll.GetComponent<HexagonController>();
            if(controller)
                controllers.Add(controller);
        }

        return controllers.OrderByDescending(controller => 
            controller.transform.position.y).ToList();
    }
}
