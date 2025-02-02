﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Move
{
    MoveData _base;

    public MoveData Base
    {
        get { return _base; }
        set { _base = value; }
    }
    public int Mana { get; set; }

    public Move(MoveData cBase)
    {
        Base = cBase;
        Mana = cBase.Mana;
    }

    /// <summary>
    /// Constructor to restore the save data of Move class.
    /// </summary>
    /// <param name="saveData"></param>
    public Move(MoveSaveData saveData)
    {
        Base = MoveDB.GetMoveByName(saveData.name);
        Mana = saveData.mana;
    }

    public MoveSaveData GetSaveData()
    {
        var saveData = new MoveSaveData()
        {
            name = Base.Name,
            mana = Base.Mana
        };

        return saveData;
    }

    //Implementation of restoring PP item.
    public void IncreaseMana(int amount)
    {
        Mana = Mathf.Clamp(Mana + amount, 0, Base.Mana);
    }
}

[System.Serializable]
public class MoveSaveData
{
    public string name;
    public int mana;
}