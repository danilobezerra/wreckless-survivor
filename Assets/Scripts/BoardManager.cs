using Cinemachine;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_WEBGL
using UnityEngine.Networking;
#endif

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private GameObject _healthPrefab;

    [SerializeField] private Tilemap[] _tilemaps;
    [SerializeField] private Tile[] _tiles;

    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f); // O.o

        var currentLevel = GameManager.Instance.CurrentLevel;
        var path = Path.Combine(Application.streamingAssetsPath, $"Levels/{currentLevel:D2}.txt");

        string input = null;

#if UNITY_WEBGL
        var www = UnityWebRequest.Get(path);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.LogError(www.error);
        } else {
            input = www.downloadHandler.text;
        }
#else
        try {
            var file = new FileInfo(path);
            using (var reader = file.OpenText()) {
                input = reader.ReadToEnd();
            }
        } catch (FileNotFoundException e) {
            Debug.LogError(e);
            GameManager.Instance.GameOverByContent();
        }

        yield return new WaitForEndOfFrame();
#endif
        if (input != null) {
            Init(input);
        }
    }

    private void Init(string input)
    {
        var rows = input.Split('\n');
        var board = new int[rows.Length, rows.Length];

        for (int i = 0; i < rows.Length; i++) {
            var columns = Regex.Split(rows[i], @"\s+")
                .Where(str => str != string.Empty)
                .ToArray();

            for (int j = 0; j < columns.Length; j++) {
                board[i, j] = int.Parse(columns[j]);
            }
        }

        BuildTilemap(board);
    }

    private void BuildTilemap(int[,] board)
    {
        for (int i = 0; i < board.GetLength(0); i++) {
            for (int j = 0; j < board.GetLength(1); j++) {
                BuildTile(board, i, j);
            }
        }
    }

    private void BuildTile(int[,] board, int i, int j)
    {
        var lastIndexOfRows = board.GetLength(0) - 1;
        var tilemapPosition = new Vector3Int(j, lastIndexOfRows - i, 0);

        int value = board[i, j];

        switch (value) {
            case 0: case 1: case 2: // Floor, Wall, Goals
                _tilemaps[value].SetTile(tilemapPosition, _tiles[value]);
                break;
            case 3: // Player
                _tilemaps[0].SetTile(tilemapPosition, _tiles[0]);

                var playerPosition = _tilemaps[0].GetCellCenterWorld(tilemapPosition);
                var player = Instantiate(_playerPrefab, playerPosition, Quaternion.identity);

                _virtualCamera.Follow = player.transform;
                break;
            case 4: // Coin
                _tilemaps[0].SetTile(tilemapPosition, _tiles[0]);

                var coinPosition = _tilemaps[0].GetCellCenterWorld(tilemapPosition);
                Instantiate(_coinPrefab, coinPosition, Quaternion.identity);
                break;
            case 5: // Health
                _tilemaps[0].SetTile(tilemapPosition, _tiles[0]);

                var healthPosition = _tilemaps[0].GetCellCenterWorld(tilemapPosition);
                Instantiate(_healthPrefab, healthPosition, Quaternion.identity);
                break;
        }
    }
}
