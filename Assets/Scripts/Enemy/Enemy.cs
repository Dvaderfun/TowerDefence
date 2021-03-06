﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Attributes")]
    public float startSpeed = 10f; //скорость врага
    
    [HideInInspector]
    public float speed; //скорость врага
    public float startHealth;
    private float health; //кол-во жизней
    public int shards = 10; //количество денег получаемых с врага
    private Transform target; //точка куда двигаться
    private int wavepointIndex = 0;
    
    [Header("Unity Setup Fields")]
    public GameObject enemyDieEffect;
    private float dieEffectLive = 0.5f; //продолжительность существования эффекта смерти врага

    public Image healthBar;
    void Start()
    {
        target = WayPoints.points[0]; //Изначально таргетирование на первую точку
        speed = startSpeed;
        health = startHealth * WaveSpawner.healthMult; //стартовые жизни с множителем
    }

    void Update()
    {
        
        Vector3 dir = target.position - transform.position; //определение расстояния до чекпоинта
        transform.Translate(dir.normalized * speed * Time.deltaTime); //движение в сторону точки

        //если враг достиг контрольной точки, берем следующую
        if(Vector3.Distance(transform.position, target.position) <= 0.1f) {
            GetNextWaypoint();
        }
        speed = startSpeed;
    }

    public void TakeDamage(float damage) {
        health -= damage;

        healthBar.fillAmount = health / (startHealth * WaveSpawner.healthMult);
        if (health <= 0) {
            Die();
        }
    }

    public void Slow(float slowPct) {
        speed = startSpeed * (1f - slowPct);
    }

    void Die() {
        PlayerStats.Money += shards; //получение денег
        WaveSpawner.enemiesAlive--;
        GameObject dieEffect = Instantiate(enemyDieEffect, transform.position, transform.rotation); //активация эффекта смерти
        Destroy(gameObject); //уничтожение объекта
        Destroy(dieEffect, dieEffectLive); //уничтожение эффекта смерти
    }

    void GetNextWaypoint() {
                //проверка, если враг достиг последней контрольной точки - уничтожение
        if (wavepointIndex >= WayPoints.points.Length - 1) {
            EndPath();
            return;
        }
        wavepointIndex++;
        target = WayPoints.points[wavepointIndex];
    }

    void EndPath() {
        PlayerStats.Lives--; //при достижении последней точки, снятие жизни игрока и уничтожение врага
        WaveSpawner.enemiesAlive--;
        Destroy(gameObject);
    }
}
