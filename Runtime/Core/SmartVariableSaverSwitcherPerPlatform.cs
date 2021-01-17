using System.Collections;
using System.Collections.Generic;
using SmartVariables;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Variables/VariableSavers/SwitcherPerPlatform")]
public class SmartVariableSaverSwitcherPerPlatform : SmartVariableSaverBase
{
    public SmartVariableSaverBase EditorWinSaver;
    public SmartVariableSaverBase EditorOsxSaver;
    public SmartVariableSaverBase EditorLinuxSaver;
    public SmartVariableSaverBase WindowsSaver;
    public SmartVariableSaverBase AndroidSaver;
    public SmartVariableSaverBase SwitchSaver;
    public SmartVariableSaverBase OsxSaver;
    public SmartVariableSaverBase LinuxSaver;
    public SmartVariableSaverBase IosSaver;
    public SmartVariableSaverBase Ps4Saver;
    public SmartVariableSaverBase XboxOneSaver;
    
    
    public override object GetSavedVariableValue(int variableId)
    {
#if UNITY_EDITOR_OSX
        return EditorOsxSaver?.GetSavedVariableValue(variableId);
#elif UNITY_EDITOR_WIN
        return EditorWinSaver?.GetSavedVariableValue(variableId);
#elif UNITY_EDITOR_LINUX
        return EditorLinuxSaver?.GetSavedVariableValue(variableId);
#elif UNITY_STANDALONE_OSX
        return OsxSaver?.GetSavedVariableValue(variableId);
#elif UNITY_STANDALONE_LINUX
        return LinuxSaver?.GetSavedVariableValue(variableId);
#elif UNITY_STANDALONE_WIN
        return WindowsSaver?.GetSavedVariableValue(variableId);
#elif UNITY_ANDROID
        return AndroidSaver?.GetSavedVariableValue(variableId);
#elif UNITY_SWITCH
        return SwitchSaver?.GetSavedVariableValue(variableId);
#elif UNITY_IOS
        return IosSaver?.GetSavedVariableValue(variableId);
#elif UNITY_PS4
        return Ps4Saver?.GetSavedVariableValue(variableId);
#elif UNITY_XBOXONE
        return XboxOneSaver?.GetSavedVariableValue(variableId);
#endif
    }

    public override void LoadVariables()
    {
#if UNITY_EDITOR_OSX
        EditorOsxSaver?.LoadVariables();
#elif UNITY_EDITOR_WIN
        EditorWinSaver?.LoadVariables();
#elif UNITY_EDITOR_LINUX
        EditorLinuxSaver?.LoadVariables();
#elif UNITY_STANDALONE_OSX
        OsxSaver?.LoadVariables();
#elif UNITY_STANDALONE_LINUX
        LinuxSaver?.LoadVariables();
#elif UNITY_STANDALONE_WIN
        WindowsSaver?.LoadVariables();
#elif UNITY_ANDROID
        AndroidSaver?.LoadVariables();
#elif UNITY_SWITCH
        SwitchSaver?.LoadVariables();
#elif UNITY_IOS
        IosSaver?.LoadVariables();
#elif UNITY_PS4
        Ps4Saver?.LoadVariables();
#elif UNITY_XBOXONE
        XboxOneSaver?.LoadVariables();
#endif
    }

    public override void SaveQueuedVariables()
    {
#if UNITY_EDITOR_OSX
        EditorOsxSaver?.SaveQueuedVariables();
#elif UNITY_EDITOR_WIN
        EditorWinSaver?.SaveQueuedVariables();
#elif UNITY_EDITOR_LINUX
        EditorLinuxSaver?.SaveQueuedVariables();
#elif UNITY_STANDALONE_OSX
        OsxSaver?.SaveQueuedVariables();
#elif UNITY_STANDALONE_LINUX
        LinuxSaver?.SaveQueuedVariables();
#elif UNITY_STANDALONE_WIN
        WindowsSaver?.SaveQueuedVariables();
#elif UNITY_ANDROID
        AndroidSaver?.SaveQueuedVariables();
#elif UNITY_SWITCH
        SwitchSaver?.SaveQueuedVariables();
#elif UNITY_IOS
        IosSaver?.SaveQueuedVariables();
#elif UNITY_PS4
        Ps4Saver?.SaveQueuedVariables();
#elif UNITY_XBOXONE
        XboxOneSaver?.SaveQueuedVariables();
#endif
    }

    public override void AddVariableToSaveQueue(SmartReferenceBase variable)
    {
#if UNITY_EDITOR_OSX
        EditorOsxSaver?.AddVariableToSaveQueue(variable);
#elif UNITY_EDITOR_WIN
        EditorWinSaver?.AddVariableToSaveQueue(variable);
#elif UNITY_EDITOR_LINUX
        EditorLinuxSaver?.AddVariableToSaveQueue(variable);
#elif UNITY_STANDALONE_OSX
        OsxSaver?.AddVariableToSaveQueue(variable);
#elif UNITY_STANDALONE_LINUX
        LinuxSaver?.AddVariableToSaveQueue(variable);
#elif UNITY_STANDALONE_WIN
        WindowsSaver?.AddVariableToSaveQueue(variable);
#elif UNITY_ANDROID
        AndroidSaver?.AddVariableToSaveQueue(variable);
#elif UNITY_SWITCH
        SwitchSaver?.AddVariableToSaveQueue(variable);
#elif UNITY_IOS
        IosSaver?.AddVariableToSaveQueue(variable);
#elif UNITY_PS4
        Ps4Saver?.AddVariableToSaveQueue(variable);
#elif UNITY_XBOXONE
        XboxOneSaver?.AddVariableToSaveQueue(variable);
#endif
    }
}
