using UnityEngine;

/// <summary>
/// Enemy - Nhân vật địch
/// Inherit từ Character, chỉnh sửa FindClosestTarget để tìm Player
/// </summary>
public class Enemy : Character
{
    /// <summary>
    /// Override: Find closest PLAYER target (not enemy)
    /// </summary>
    protected override void FindClosestTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0)
        {
            currentTarget = null;
            return;
        }

        Character closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            if (player == null) continue;
            Character playerChar = player.GetComponent<Character>();
            if (playerChar == null || playerChar.IsDead()) continue;

            float dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = playerChar;
            }
        }

        currentTarget = closest;
    }
}
