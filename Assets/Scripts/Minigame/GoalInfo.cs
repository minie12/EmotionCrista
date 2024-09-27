using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ERASE!

public class GoalInfo : MonoBehaviour
{
    private BoardManager board;
    private MiniManager mini;

    // ui, sprite
    public SpriteRenderer goalSprite;
    private List<Sprite[]> goal_sprites = new List<Sprite[]>(); // 0: 목표광물 2, 1: 목표광물 3, 2: 목표광물 4

    // goal board
    private int[,] goal2_e = new int[6,2] {{1, 1}, {1, 0}, {0, -1}, {-1, 0}, {-1, 1}, {0, 1}};
    private int[,] goal2_o = new int[6,2] {{1, 0}, {1, -1}, {0, -1}, {-1, -1}, {-1, 0}, {0, 1}};

    private int[,,] goal3_e = new int[33, 2, 2] {{{1, 1},{1, 0}}, {{-1, 0},{0, -1}}, {{-1, 1},{0, 1}}, // 1
                                                 {{1, 1},{2, 0}}, {{-1, 0},{1, 0}}, {{-2, 0},{-1, 1}}, // 2
                                                 {{1, 1},{2, 1}}, {{-1, 0},{1, 1}}, {{-2, -1},{-1, 0}}, // 3
                                                 {{1, 1},{1, 2}}, {{-1, 0},{0, 1}}, {{-1, -1},{0, -1}}, // 4
                                                 {{0, -1},{1, 0}}, {{0, 1},{1, 1}}, {{-1, 1},{-1, 0}}, // 5
                                                 {{1, 0},{2, 0}}, {{-1, 1},{1, 1}}, {{-2, 0},{-1, 0}}, // 6
                                                 {{-1, 0},{-1, -1}}, {{0, -1},{1, 1}}, {{0, 1},{1, 2}}, // 7
                                                 {{1, 0},{2, -1}}, {{-1, 1},{1, 0}}, {{-2, 1},{-1, 1}}, // 8
                                                 {{1, 0},{1, -1}}, {{-1, 1},{0, -1}}, {{-1, 2},{0, 1}}, // 9
                                                 {{0, -1},{1, -1}}, {{0, 1},{1, 0}}, {{-1, 2},{-1, 1}}, // 10
                                                 {{0, -1},{0, -2}}, {{0, 1},{0, -1}}, {{0, 2},{0, 1}} // 11
                                                };
    private int[,,] goal3_o = new int[33, 2, 2] {{{1, 0},{1, -1}}, {{-1, -1},{0, -1}}, {{-1, 0},{0, 1}}, // 1
                                                 {{1, 0},{2, 0}}, {{-1, -1},{1, -1}}, {{-2, 0},{-1, 0}}, // 2
                                                 {{1, 0},{2,1}}, {{-1, -1},{1, 0}}, {{-2, -1},{-1, -1}}, // 3
                                                 {{1, 0},{1, 1}}, {{-1, -1},{0, 1}}, {{-1, -2},{0, -1}}, // 4
                                                 {{0, -1},{1, -1}}, {{0, 1},{1, 0}}, {{-1, 0},{-1, -1}}, // 5
                                                 {{1, -1},{2, 0}}, {{-1, 0},{1, 0}}, {{-2, 0},{-1, -1}}, // 6
                                                 {{-1, -1},{-1, -2}}, {{0, -1},{1, 0}}, {{0, 1},{1, 1}}, // 7
                                                 {{1, -1},{2, -1}}, {{-1, 0},{1, -1}}, {{-2, 1},{-1, 0}}, // 8
                                                 {{1, -1},{1, -2}}, {{-1, 0},{0, -1}}, {{-1, 1},{0, 1}}, // 9
                                                 {{0, -1},{1, -2}}, {{0, 1},{1, -1}}, {{-1, 1},{-1, 0}}, // 10
                                                 {{0, -1},{0, -2}}, {{0, 1},{0, -1}}, {{0, 2},{0, 1}} // 11
                                                };

    // 엑셀 파일에서 배열 정보 가져오기
    // https://docs.google.com/spreadsheets/d/16OYi7HfGp9FEu74G_PZ5SK3ORZUdwKDKbf3u4YxKZN0/edit?usp=sharing
    private int[,,] goal4_e = new int[120, 3, 2] { { { 1, 1 }, { 0, 1 }, { 1, 1 + 1 } }, { { 0, 1 }, { -1, 1 - 1 }, { -1, 1 } }, { { 0, -1 }, { -1, 1 - 1 }, { -1, 1 - 2 } }, { { 0, -1 }, { 1, 1 }, { 1, 1 - 1 } }, { { 0, 1 }, { -1, 1 }, { -1, 1 + 1 } }, { { 0, 1 }, { 1, 1 - 1 }, { 1, 1 } }, { { 0, -1 }, { 1, 1 - 1 }, { 1, 1 - 2 } }, { { 0, -1 }, { -1, 1 }, { -1, 1 - 1 } }, { { 0, 1 }, { -1, 1 }, { -2, 0 } }, { { 1, 1 }, { 2, 1 }, { 2, 0 } }, { { 0, -1 }, { -1, 1 - 1 }, { -2, -1 } }, { { 1, 1 }, { 1, 1 - 1 }, { -1, 1 - 1 } }, { { 0, -1 }, { -1, 1 - 1 }, { -2, 0 } }, { { 1, 1 - 1 }, { 2, 0 }, { 2, -1 } }, { { 0, 1 }, { -1, 1 }, { -2, 1 } }, { { -1, 1 }, { 1, 1 }, { 1, 1 - 1 } }, { { -1, 1 }, { 0, 1 }, { 1, 1 + 1 } }, { { 1, 1 - 1 }, { 1, 1 }, { 2, 1 } }, { { -1, 1 - 1 }, { -2, -1 }, { -1, 1 - 2 } }, { { -1, 1 - 1 }, { 0, -1 }, { 1, 1 } }, { { -1, 1 - 1 }, { 0, -1 }, { 1, 1 - 2 } }, { { 1, 1 }, { 1, 1 - 2 }, { 2, -2 } }, { { -1, 1 }, { -2, 2 }, { -1, 1 + 2 } }, { { 0, 1 }, { -1, 1 }, { 1, 1 - 1 } }, { { 1, 1 }, { 1, 1 - 1 }, { 1, 1 - 2 } }, { { -1, 1 - 1 }, { 0, -1 }, { 0, -2 } }, { { 0, 1 }, { 0, 2 }, { -1, 1 + 1 } }, { { 0, 1 }, { -1, 1 }, { 0, -1 } }, { { 1, 1 + 1 }, { 1, 1 }, { 1, 1 - 1 } }, { { -1, 1 }, { 0, 1 }, { 0, 2 } }, { { 0, -1 }, { -1, 1 - 2 }, { 0, -2 } }, { { 0, 1 }, { -1, 1 - 1 }, { 0, -1 } }, { { 1, 1 }, { 1, 1 - 1 }, { 2, 0 } }, { { -1, 1 }, { 0, 1 }, { 1, 1 } }, { { -1, 1 }, { -2, 0 }, { -1, 1 - 1 } }, { { -1, 1 - 1 }, { 0, -1 }, { 1, 1 - 1 } }, { { 0, 1 }, { 1, 1 }, { 2, 0 } }, { { 0, -1 }, { 1, 1 - 1 }, { 2, -1 } }, { { -1, 1 }, { -2, 1 }, { -2, 0 } }, { { -1, 1 }, { -1, 1 - 1 }, { 1, 1 - 1 } }, { { 1, 1 }, { 0, 1 }, { -1, 1 + 1 } }, { { -1, 1 - 1 }, { -1, 1 }, { -2, 1 } }, { { 1, 1 - 1 }, { 2, -1 }, { 1, 1 - 2 } }, { { -1, 1 }, { 1, 1 - 1 }, { 0, -1 } }, { { 0, 1 }, { -1, 1 + 1 }, { -2, 2 } }, { { -1, 1 }, { -2, 1 }, { 0, -1 } }, { { 1, 1 - 1 }, { 2, -1 }, { 2, -2 } }, { { -1, 1 }, { 1, 1 - 1 }, { 1, 1 - 2 } }, { { -1, 1 - 1 }, { -2, 0 }, { -3, 1 } }, { { 1, 1 }, { -1, 1 }, { -2, 1 } }, { { 1, 1 - 1 }, { 2, -1 }, { 3, 1 - 1 } }, { { 1, 1 - 1 }, { 2, 0 }, { -1, 1 } }, { { 0, -1 }, { 1, 1 - 2 }, { 2, -2 } }, { { 0, 1 }, { 1, 1 - 1 }, { 2, -1 } }, { { -1, 1 }, { -2, 1 }, { -2, 2 } }, { { 1, 1 - 1 }, { -1, 1 }, { -1, 1 + 1 } }, { { 1, 1 }, { 2, 0 }, { 3, 1 - 1 } }, { { -1, 1 - 1 }, { 1, 1 - 1 }, { 2, -1 } }, { { -1, 1 }, { -2, 1 }, { -3, 1 } }, { { 1, 1 - 1 }, { -1, 1 }, { -2, 0 } }, { { 1, 1 - 1 }, { 2, -1 }, { 3, 1 - 2 } }, { { -1, 1 }, { 1, 1 - 1 }, { 2, -1 } }, { { -1, 1 }, { -2, 1 }, { -3, 1 + 1 } }, { { 1, 1 - 1 }, { -1, 1 }, { -2, 1 } }, { { -1, 1 - 1 }, { -2, -1 }, { -3, 1 - 2 } }, { { 1, 1 }, { -1, 1 - 1 }, { -2, -1 } }, { { 1, 1 }, { 2, 1 }, { 3, 1 + 1 } }, { { -1, 1 - 1 }, { 1, 1 }, { 2, 1 } }, { { -1, 1 }, { -2, 0 }, { -3, 1 - 1 } }, { { 1, 1 - 1 }, { -1, 1 - 1 }, { -2, -1 } }, { { 1, 1 }, { 2, 1 }, { 3, 1 } }, { { -1, 1 - 1 }, { 1, 1 }, { 2, 0 } }, { { 1, 1 - 1 }, { 2, 0 }, { 3, 1 } }, { { -1, 1 }, { 1, 1 }, { 2, 1 } }, { { -1, 1 - 1 }, { -2, -1 }, { -3, 1 - 1 } }, { { 1, 1 }, { -1, 1 - 1 }, { -2, 0 } }, { { 0, -1 }, { -1, 1 - 2 }, { -2, -2 } }, { { 0, 1 }, { -1, 1 - 1 }, { -2, -1 } }, { { 1, 1 }, { 2, 1 }, { 2, 2 } }, { { -1, 1 - 1 }, { 1, 1 }, { 1, 1 + 1 } }, { { 0, 1 }, { 1, 1 + 1 }, { 2, 2 } }, { { 0, -1 }, { 1, 1 }, { 2, 1 } }, { { -1, 1 - 1 }, { -2, -1 }, { -2, -2 } }, { { 1, 1 }, { -1, 1 - 1 }, { -1, 1 - 2 } }, { { 0, -1 }, { 1, 1 - 1 }, { 2, 0 } }, { { 0, 1 }, { 1, 1 }, { 2, 1 } }, { { -1, 1 - 1 }, { -2, -1 }, { -2, 0 } }, { { -1, 1 }, { -1, 1 - 1 }, { 1, 1 } }, { { 1, 1 - 1 }, { 0, -1 }, { -1, 1 - 2 } }, { { -1, 1 }, { -1, 1 - 1 }, { -2, -1 } }, { { 1, 1 }, { 2, 1 }, { 1, 1 + 1 } }, { { 0, 1 }, { 1, 1 }, { -1, 1 - 1 } }, { { -1, 1 - 1 }, { -1, 1 }, { -1, 1 + 1 } }, { { 0, 1 }, { 0, 2 }, { 1, 1 } }, { { 0, -1 }, { 0, -2 }, { 1, 1 - 2 } }, { { 0, 1 }, { 0, -1 }, { 1, 1 - 1 } }, { { -1, 1 }, { -1, 1 - 1 }, { -1, 1 - 2 } }, { { 0, -1 }, { 0, -2 }, { 1, 1 - 1 } }, { { 0, 1 }, { 0, 2 }, { 1, 1 + 1 } }, { { 0, 1 }, { 0, -1 }, { 1, 1 } }, { { 0, -1 }, { 0, -2 }, { 0, -3 } }, { { 0, 1 }, { 0, -1 }, { 0, -2 } }, { { 0, 1 }, { 0, 2 }, { 0, 3 } }, { { 0, -1 }, { 0, 1 }, { 0, 2 } }, { { -1, 1 }, { -1, 1 + 1 }, { -1, 1 + 2 } }, { { 0, 1 }, { 0, 2 }, { 1, 1 - 1 } }, { { 0, -1 }, { 0, -2 }, { 1, 1 - 3 } }, { { 0, 1 }, { 0, -1 }, { 1, 1 - 2 } }, { { 1, 1 - 1 }, { 1, 1 - 2 }, { 1, 1 - 3 } }, { { 0, -1 }, { 0, -2 }, { -1, 1 } }, { { 0, 1 }, { 0, 2 }, { -1, 1 + 2 } }, { { 0, -1 }, { 0, 1 }, { -1, 1 + 1 } }, { { -1, 1 - 1 }, { -1, 1 - 2 }, { -1, 1 - 3 } }, { { -1, 1 - 1 }, { -1, 1 - 2 }, { -1, 1 - 3 } }, { { 0, 1 }, { 0, 2 }, { 1, 1 + 2 } }, { { 0, -1 }, { 0, 1 }, { 1, 1 + 1 } }, { { 1, 1 }, { 1, 1 + 1 }, { 1, 1 + 2 } }, { { 0, 1 }, { 0, 2 }, { -1, 1 - 1 } }, { { 0, -1 }, { 0, -2 }, { -1, 1 - 3 } }, { { 0, 1 }, { 0, -1 }, { -1, 1 - 2 } } };
    private int[,,] goal4_o = new int[120, 3, 2] { { { 1, 0 }, { 0, 1 }, { 1, 0 + 1 } }, { { 0, 1 }, { -1, 0 - 1 }, { -1, 0 } }, { { 0, -1 }, { -1, 0 - 1 }, { -1, 0 - 2 } }, { { 0, -1 }, { 1, 0 }, { 1, 0 - 1 } }, { { 0, 1 }, { -1, 0 }, { -1, 0 + 1 } }, { { 0, 1 }, { 1, 0 - 1 }, { 1, 0 } }, { { 0, -1 }, { 1, 0 - 1 }, { 1, 0 - 2 } }, { { 0, -1 }, { -1, 0 }, { -1, 0 - 1 } }, { { 0, 1 }, { -1, 0 }, { -2, 0 } }, { { 1, 0 }, { 2, 1 }, { 2, 0 } }, { { 0, -1 }, { -1, 0 - 1 }, { -2, -1 } }, { { 1, 0 }, { 1, 0 - 1 }, { -1, 0 - 1 } }, { { 0, -1 }, { -1, 0 - 1 }, { -2, 0 } }, { { 1, 0 - 1 }, { 2, 0 }, { 2, -1 } }, { { 0, 1 }, { -1, 0 }, { -2, 1 } }, { { -1, 0 }, { 1, 0 }, { 1, 0 - 1 } }, { { -1, 0 }, { 0, 1 }, { 1, 0 + 1 } }, { { 1, 0 - 1 }, { 1, 0 }, { 2, 1 } }, { { -1, 0 - 1 }, { -2, -1 }, { -1, 0 - 2 } }, { { -1, 0 - 1 }, { 0, -1 }, { 1, 0 } }, { { -1, 0 - 1 }, { 0, -1 }, { 1, 0 - 2 } }, { { 1, 0 }, { 1, 0 - 2 }, { 2, -2 } }, { { -1, 0 }, { -2, 2 }, { -1, 0 + 2 } }, { { 0, 1 }, { -1, 0 }, { 1, 0 - 1 } }, { { 1, 0 }, { 1, 0 - 1 }, { 1, 0 - 2 } }, { { -1, 0 - 1 }, { 0, -1 }, { 0, -2 } }, { { 0, 1 }, { 0, 2 }, { -1, 0 + 1 } }, { { 0, 1 }, { -1, 0 }, { 0, -1 } }, { { 1, 0 + 1 }, { 1, 0 }, { 1, 0 - 1 } }, { { -1, 0 }, { 0, 1 }, { 0, 2 } }, { { 0, -1 }, { -1, 0 - 2 }, { 0, -2 } }, { { 0, 1 }, { -1, 0 - 1 }, { 0, -1 } }, { { 1, 0 }, { 1, 0 - 1 }, { 2, 0 } }, { { -1, 0 }, { 0, 1 }, { 1, 0 } }, { { -1, 0 }, { -2, 0 }, { -1, 0 - 1 } }, { { -1, 0 - 1 }, { 0, -1 }, { 1, 0 - 1 } }, { { 0, 1 }, { 1, 0 }, { 2, 0 } }, { { 0, -1 }, { 1, 0 - 1 }, { 2, -1 } }, { { -1, 0 }, { -2, 1 }, { -2, 0 } }, { { -1, 0 }, { -1, 0 - 1 }, { 1, 0 - 1 } }, { { 1, 0 }, { 0, 1 }, { -1, 0 + 1 } }, { { -1, 0 - 1 }, { -1, 0 }, { -2, 1 } }, { { 1, 0 - 1 }, { 2, -1 }, { 1, 0 - 2 } }, { { -1, 0 }, { 1, 0 - 1 }, { 0, -1 } }, { { 0, 1 }, { -1, 0 + 1 }, { -2, 2 } }, { { -1, 0 }, { -2, 1 }, { 0, -1 } }, { { 1, 0 - 1 }, { 2, -1 }, { 2, -2 } }, { { -1, 0 }, { 1, 0 - 1 }, { 1, 0 - 2 } }, { { -1, 0 - 1 }, { -2, 0 }, { -3, 0 } }, { { 1, 0 }, { -1, 0 }, { -2, 1 } }, { { 1, 0 - 1 }, { 2, -1 }, { 3, 0 - 1 } }, { { 1, 0 - 1 }, { 2, 0 }, { -1, 0 } }, { { 0, -1 }, { 1, 0 - 2 }, { 2, -2 } }, { { 0, 1 }, { 1, 0 - 1 }, { 2, -1 } }, { { -1, 0 }, { -2, 1 }, { -2, 2 } }, { { 1, 0 - 1 }, { -1, 0 }, { -1, 0 + 1 } }, { { 1, 0 }, { 2, 0 }, { 3, 0 - 1 } }, { { -1, 0 - 1 }, { 1, 0 - 1 }, { 2, -1 } }, { { -1, 0 }, { -2, 1 }, { -3, 0 } }, { { 1, 0 - 1 }, { -1, 0 }, { -2, 0 } }, { { 1, 0 - 1 }, { 2, -1 }, { 3, 0 - 2 } }, { { -1, 0 }, { 1, 0 - 1 }, { 2, -1 } }, { { -1, 0 }, { -2, 1 }, { -3, 0 + 1 } }, { { 1, 0 - 1 }, { -1, 0 }, { -2, 1 } }, { { -1, 0 - 1 }, { -2, -1 }, { -3, 0 - 2 } }, { { 1, 0 }, { -1, 0 - 1 }, { -2, -1 } }, { { 1, 0 }, { 2, 1 }, { 3, 0 + 1 } }, { { -1, 0 - 1 }, { 1, 0 }, { 2, 1 } }, { { -1, 0 }, { -2, 0 }, { -3, 0 - 1 } }, { { 1, 0 - 1 }, { -1, 0 - 1 }, { -2, -1 } }, { { 1, 0 }, { 2, 1 }, { 3, 0 } }, { { -1, 0 - 1 }, { 1, 0 }, { 2, 0 } }, { { 1, 0 - 1 }, { 2, 0 }, { 3, 0 } }, { { -1, 0 }, { 1, 0 }, { 2, 1 } }, { { -1, 0 - 1 }, { -2, -1 }, { -3, 0 - 1 } }, { { 1, 0 }, { -1, 0 - 1 }, { -2, 0 } }, { { 0, -1 }, { -1, 0 - 2 }, { -2, -2 } }, { { 0, 1 }, { -1, 0 - 1 }, { -2, -1 } }, { { 1, 0 }, { 2, 1 }, { 2, 2 } }, { { -1, 0 - 1 }, { 1, 0 }, { 1, 0 + 1 } }, { { 0, 1 }, { 1, 0 + 1 }, { 2, 2 } }, { { 0, -1 }, { 1, 0 }, { 2, 1 } }, { { -1, 0 - 1 }, { -2, -1 }, { -2, -2 } }, { { 1, 0 }, { -1, 0 - 1 }, { -1, 0 - 2 } }, { { 0, -1 }, { 1, 0 - 1 }, { 2, 0 } }, { { 0, 1 }, { 1, 0 }, { 2, 1 } }, { { -1, 0 - 1 }, { -2, -1 }, { -2, 0 } }, { { -1, 0 }, { -1, 0 - 1 }, { 1, 0 } }, { { 1, 0 - 1 }, { 0, -1 }, { -1, 0 - 2 } }, { { -1, 0 }, { -1, 0 - 1 }, { -2, -1 } }, { { 1, 0 }, { 2, 1 }, { 1, 0 + 1 } }, { { 0, 1 }, { 1, 0 }, { -1, 0 - 1 } }, { { -1, 0 - 1 }, { -1, 0 }, { -1, 0 + 1 } }, { { 0, 1 }, { 0, 2 }, { 1, 0 } }, { { 0, -1 }, { 0, -2 }, { 1, 0 - 2 } }, { { 0, 1 }, { 0, -1 }, { 1, 0 - 1 } }, { { -1, 0 }, { -1, 0 - 1 }, { -1, 0 - 2 } }, { { 0, -1 }, { 0, -2 }, { 1, 0 - 1 } }, { { 0, 1 }, { 0, 2 }, { 1, 0 + 1 } }, { { 0, 1 }, { 0, -1 }, { 1, 0 } }, { { 0, -1 }, { 0, -2 }, { 0, -3 } }, { { 0, 1 }, { 0, -1 }, { 0, -2 } }, { { 0, 1 }, { 0, 2 }, { 0, 3 } }, { { 0, -1 }, { 0, 1 }, { 0, 2 } }, { { -1, 0 }, { -1, 0 + 1 }, { -1, 0 + 2 } }, { { 0, 1 }, { 0, 2 }, { 1, 0 - 1 } }, { { 0, -1 }, { 0, -2 }, { 1, 0 - 3 } }, { { 0, 1 }, { 0, -1 }, { 1, 0 - 2 } }, { { 1, 0 - 1 }, { 1, 0 - 2 }, { 1, 0 - 3 } }, { { 0, -1 }, { 0, -2 }, { -1, 0 } }, { { 0, 1 }, { 0, 2 }, { -1, 0 + 2 } }, { { 0, -1 }, { 0, 1 }, { -1, 0 + 1 } }, { { -1, 0 - 1 }, { -1, 0 - 2 }, { -1, 0 - 3 } }, { { -1, 0 - 1 }, { -1, 0 - 2 }, { -1, 0 - 3 } }, { { 0, 1 }, { 0, 2 }, { 1, 0 + 2 } }, { { 0, -1 }, { 0, 1 }, { 1, 0 + 1 } }, { { 1, 0 }, { 1, 0 + 1 }, { 1, 0 + 2 } }, { { 0, 1 }, { 0, 2 }, { -1, 0 - 1 } }, { { 0, -1 }, { 0, -2 }, { -1, 0 - 3 } }, { { 0, 1 }, { 0, -1 }, { -1, 0 - 2 } } };

    // info
    private int goalGemCnt = 2;
    private int goalIdx = -1;

    // crush gems info
    public List<List<int>> crushedGems = new List<List<int>>();

    // DEBUG PURPOSE!!!!
    //public Dropdown goalOption;
    //------------------------------

    private void Awake()
    {
        // 목표광물 경로
        string minigame_locate = "Minigame/goals";

        // 목표광물 sprites 불러오기
        for (int i = 2; i <= 4; i++)
        {
            Sprite[] goal_temp_sprites = Resources.LoadAll<Sprite>($"{minigame_locate}/{i}");
            goal_sprites.Add(goal_temp_sprites);
        }
    }

    void Start(){
        board = gameObject.GetComponent<BoardManager>();
        mini = GameObject.Find("MiniManager").GetComponent<MiniManager>();
    }

    public int GetGoalNumber()
    {
        return this.goalIdx + 1;
    }

    public void SetGoal(int unit, int goalNum = 0){
        goalGemCnt = unit + 2;
        int total_goal_cnt = goal_sprites[unit].GetLength(0);

        if (unit < 0 || unit > 4 || goalNum > total_goal_cnt)
        {
            return;
        }

        if (goalNum > 0)
        {
            // goalNum : 1부터 시작, goalIdx: 0부터 시작
            goalIdx = goalNum - 1;
        }
        else if (goalNum == 0)
        {
            goalNum = goalIdx + 1;
            while ((goalNum - 1) == goalIdx)
            {
                goalNum = Random.Range(1, total_goal_cnt + 1);
            }

            goalIdx = goalNum - 1;
        }

        goalSprite.sprite = goal_sprites[unit][goalIdx];
        Debug.Log("목표광물 번호 !!!!!!!!" + goalIdx);
    }

    // 광물 한 개에 대해 클릭한 광물과 목표 광물인지 비교하는 함수
    private void CheckGoalOneGem(ref List<bool> results, int column, int row, int goalColor)
    {
        if (board.GetGem(column, row) == null)
        {
            Debug.Log("goal failed: gem is null");
            results[0] = results[1] = false;
            return;
        }
        if (mini.patternIdx == (int)PatternType.GREEN && mini.GetComponent<PatternGreen>().IsRunningGimmick(1) && !mini.GetComponent<PatternGreen>().IsInArea(column, row))
        {
            Debug.Log("goal failed: not in area (green gimmick)");
            results[1] = false;
        }
        if (board.GetGem(column, row).GetComponent<GemInfo>().bLocationFixed)
        {
            Debug.Log("goal failed: gem is fixed");
            results[1] = false;
        }
        if (goalColor != board.GetGemColor(column, row))
        {
            Debug.Log("goal failed: mismatch color");
            results[0] = results[1] = false;
        }
    }

    public bool CheckGoal(int column, int row){
        // init
        crushedGems = new List<List<int>> { new List<int> { column, row } }; // 크러쉬된 광물 위치
        List<bool> results = new List<bool> { true, true }; // 0: isMatched (목표에는 해당하는지), 1: result (실제 결과)
        int goalColor = board.GetGemColor(column, row); // 클릭한 광물 색깔
        CheckGoalOneGem(ref results, column, row, goalColor);

        if (!results[1])
        {
            crushedGems = null;
            return false;
        }

        if (goalGemCnt == 2) {
            int otherColumn, otherRow;
            if (column % 2 == 0) {
                otherColumn = column + goal2_e[goalIdx, 0];
                otherRow = row + goal2_e[goalIdx, 1];
            }
            else {
                otherColumn = column + goal2_o[goalIdx, 0];
                otherRow = row + goal2_o[goalIdx, 1];
            }
            CheckGoalOneGem(ref results, otherColumn, otherRow, goalColor);
            crushedGems.Add(new List<int> { otherColumn, otherRow });
        }
        else if (goalGemCnt == 3) {
            if (column % 2 == 0) {
                for (int i = 0; i < goal3_e.GetLength(1); i++) {
                    int row2 = row + goal3_e[goalIdx, i, 1];
                    int column2 = column + goal3_e[goalIdx, i, 0];

                    CheckGoalOneGem(ref results, column2, row2, goalColor);
                    crushedGems.Add(new List<int> { column2, row2 });
                }
            }
            else {
                for (int i = 0; i < goal3_o.GetLength(1); i++) {
                    int row2 = row + goal3_o[goalIdx, i, 1];
                    int column2 = column + goal3_o[goalIdx, i, 0];

                    CheckGoalOneGem(ref results, column2, row2, goalColor);
                    crushedGems.Add(new List<int> { column2, row2 });
                }
            }
        }
        else if (goalGemCnt == 4)
        {
            if (column % 2 == 0)
            {
                for (int i = 0; i < goal4_e.GetLength(1); i++)
                {
                    int row2 = row + goal4_e[goalIdx, i, 1];
                    int column2 = column + goal4_e[goalIdx, i, 0];

                    CheckGoalOneGem(ref results, column2, row2, goalColor);
                    crushedGems.Add(new List<int> { column2, row2 });
                }
            }
            else
            {
                for (int i = 0; i < goal4_o.GetLength(1); i++)
                {
                    int row2 = row + goal4_o[goalIdx, i, 1];
                    int column2 = column + goal4_o[goalIdx, i, 0];

                    CheckGoalOneGem(ref results, column2, row2, goalColor);
                    crushedGems.Add(new List<int> { column2, row2 });
                }
            }
        }

        if (!results[1])
        {
            crushedGems = null;
        }

        if (results[0] && !results[1])
        {
            SoundEffectManager.Instance.Play((int)SoundEffectName.DefaultBeep);
        }

        if (results[1])
        {
            for(int i = 0; i < crushedGems.Count; i++)
            {
                board.GetGem(crushedGems[i][0], crushedGems[i][1]).isCrushed = true;
            }
        }
        return results[1];
    } 

    public void EraseGems(int column, int row, bool isCrush){
        board.DelGem(column, row);

        // if is not crush, only one gems erase
        if (!isCrush)
        {
            return;
        }

        if(goalGemCnt == 2){
            if(column%2 == 0){
                int row2 = row + goal2_e[goalIdx, 1];
                int column2 = column + goal2_e[goalIdx, 0];
                board.DelGem(column2, row2);
            }
            else{
                int row2 = row + goal2_o[goalIdx, 1];
                int column2 = column + goal2_o[goalIdx, 0];
                board.DelGem(column2, row2);
            }
        } 
        else if(goalGemCnt == 3){
            if(column%2 == 0){
                for(int i = 0; i < goal3_e.GetLength(1); i++){
                    int row2 = row + goal3_e[goalIdx, i, 1];
                    int column2 = column + goal3_e[goalIdx, i, 0];
                    board.DelGem(column2, row2);
                }
            }
            else{
                for(int i = 0; i < goal3_o.GetLength(1); i++){
                    int row2 = row + goal3_o[goalIdx, i, 1];
                    int column2 = column + goal3_o[goalIdx, i, 0];
                    board.DelGem(column2, row2);
                }
            }
        }
        else if (goalGemCnt == 4)
        {
            if (column % 2 == 0)
            {
                for (int i = 0; i < goal4_e.GetLength(1); i++)
                {
                    int row2 = row + goal4_e[goalIdx, i, 1];
                    int column2 = column + goal4_e[goalIdx, i, 0];
                    board.DelGem(column2, row2);
                }
            }
            else
            {
                for (int i = 0; i < goal4_o.GetLength(1); i++)
                {
                    int row2 = row + goal4_o[goalIdx, i, 1];
                    int column2 = column + goal4_o[goalIdx, i, 0];
                    board.DelGem(column2, row2);
                }
            }
        }
    }
}
