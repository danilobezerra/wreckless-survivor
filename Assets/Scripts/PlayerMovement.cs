using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private string _tagWall = "Wall";
    [SerializeField] private string _tagGoal = "Goal";
    [SerializeField] private string _tagCoin = "Coin";
    [SerializeField] private string _tagHealth = "Health";

    [SerializeField] private AudioClip[] _sounds;

    [SerializeField] private float _moveFactor = 1f;
    [SerializeField] private float _moveSpeed = 2.5f;
    [SerializeField] private float _gridSize = 1f;

    public bool IsMoving { get; private set; }
    private void FixedUpdate()
    {
        if (!GameManager.Instance.IsGameRunning) return;
        if (IsMoving) return;

        int horizontal = (int)Input.GetAxisRaw("Horizontal");
        int vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0) {
            vertical = 0;
        }

        if (horizontal == 0 && vertical == 0) return;
        var input = new Vector2(horizontal, vertical);

        if (!CanMove(input)) return;
        StartCoroutine(Move(input));
    }

    private IEnumerator Move(Vector2 input)
    {
        IsMoving = true;
        GetComponent<AudioSource>().PlayOneShot(_sounds[0]);

        if (GameManager.Instance.IsGameRunning) {
            GameManager.Instance.DepleteLife(0.05f);
        }

        var startPosition = transform.position;
        float time = 0;

        var endPosition = new Vector2(startPosition.x + Math.Sign(input.x) * _gridSize,
            startPosition.y + Math.Sign(input.y) * _gridSize);

        while (time < 1f) {
            time += Time.deltaTime * (_moveSpeed / _gridSize) * _moveFactor;
            transform.position = Vector2.Lerp(startPosition, endPosition, time);
            yield return null;
        }

        IsMoving = false;
        yield return 0;
    }

    private bool CanMove(Vector2 direction)
    {
        var hit = Physics2D.Raycast(transform.position, direction, 1f);
        if (!hit) return true;

        if (hit.collider.CompareTag(_tagWall)) {
            // TODO: Play wall bump sound
            //GetComponent<AudioSource>().PlayOneShot(_sounds[1]);
            return false;
        }

        if (hit.collider.CompareTag(_tagGoal)) {
            if (GameManager.Instance.IsGameRunning) {
                GetComponent<AudioSource>().PlayOneShot(_sounds[2]);
                StartCoroutine(GameManager.Instance.LevelComplete());
            }

            return true;
        }

        if (hit.collider.CompareTag(_tagCoin)) {
            if (GameManager.Instance.IsGameRunning) {
                GetComponent<AudioSource>().PlayOneShot(_sounds[3]);
                GameManager.Instance.ImproveScore(100);
                // TODO: Play coin caught sound

                Destroy(hit.collider.gameObject);
            }

            return true;
        }

        if (hit.collider.CompareTag(_tagHealth)) {
            if (GameManager.Instance.IsGameRunning) {
                GetComponent<AudioSource>().PlayOneShot(_sounds[4]);
                GameManager.Instance.ReplenishLife(0.1f);
                // TODO: Play cure sound

                Destroy(hit.collider.gameObject);
            }

            return true;
        }

        return true;
    }
}
