using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    const float SHOT_SPEED = 100f;

    [SerializeField] GameObject _pollObject;
    List<GameObject> _pollList;
    List<Rigidbody> _rigidbodyList;
    int _polledAmount = 20;
    int _readyNum = 0;

    // Use this for initialization
    void Start ()
    {
        _pollList = new List<GameObject> ();
        _rigidbodyList = new List<Rigidbody> ();

        for (int i = 0; i < _polledAmount; i++)
        {
            GameObject　 _bullet = Instantiate (_pollObject, transform.position, Quaternion.identity);
            _bullet.SetActive (false);
            _bullet.transform.parent = this.gameObject.transform;
            _bullet.transform.Rotate (transform.right, 90);
            _pollList.Add (_bullet);
            _rigidbodyList.Add (_bullet.GetComponent<Rigidbody> ());
        }
    }

    public void ShootBullet ()
    {
        if (_readyNum == _polledAmount)
        {
            _readyNum = 0;
        }

        // その弾丸の初回使用時にアクティブにする
        if (!_pollList[_readyNum].activeSelf)
        {
            _pollList[_readyNum].SetActive (true);
        }
        // 再利用弾を銃口にセット
        else
        {
            _pollList[_readyNum].transform.position = transform.position;
            _pollList[_readyNum].transform.rotation = transform.rotation;
            _pollList[_readyNum].transform.Rotate (transform.right, 90);
        }

        // 発射
        _rigidbodyList[_readyNum].velocity = transform.forward * SHOT_SPEED;
        _readyNum++;
    }
}