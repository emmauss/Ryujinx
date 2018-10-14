import QtQuick 2.0
import QtQuick.Controls 2.3
import QtQuick.Layouts 1.3
import QtQuick.Dialogs 1.3
import Ryujinx 1.0

Frame {
    id: gamePadSettingsFrame

    ColumnLayout {
        anchors.fill: parent

        GroupBox {
            Layout.fillHeight: false
            title: qsTr("Options")

            ColumnLayout {
                anchors.fill: parent
                CheckBox {
                    id: enableGamePadCheckBox
                    text: "Enable GamePad"

                    checked: configModel.getValue("GamePad_Enable")

                    onCheckedChanged: {
                        configModel.setValue("GamePad_Enable", checked)
                    }
                }

                GridLayout {
                    Layout.fillHeight: true
                    Layout.fillWidth: true
                    columns: 2
                    rows: 3

                    enabled: enableGamePadCheckBox.checked

                    Label {
                        text: "GamePad Index"
                        Layout.fillHeight: false
                        Layout.fillWidth: true
                        Layout.column: 0
                        Layout.row: 0
                    }

                    Label {
                        text: "Deadzone"
                        Layout.fillWidth: true
                        Layout.column: 0
                        Layout.row: 1
                    }

                    Label {
                        text: "Trigger Threshold"
                        Layout.fillWidth: true
                        Layout.column: 0
                        Layout.row: 2
                    }

                    TextField {
                        id: gamePadIndexBox
                        horizontalAlignment: Text.AlignRight
                        Layout.preferredWidth: 80
                        Layout.fillWidth: false
                        Layout.column: 1
                        Layout.row:0
                        inputMethodHints: Qt.ImhDigitsOnly

                        text: configModel.getValue("GamePad_Index")

                        onTextEdited: {
                            configModel.setValue("GamePad_Index", text)
                        }
                    }

                    TextField {
                        id: deadzoneBox
                        horizontalAlignment: Text.AlignRight
                        Layout.preferredWidth: 80
                        Layout.column: 1
                        Layout.row:1
                        inputMethodHints: Qt.ImhFormattedNumbersOnly

                        text: configModel.getValue("GamePad_Deadzone")

                        onTextEdited: {
                            configModel.setValue("GamePad_Deadzone", text)
                        }
                    }

                    TextField {
                        id: triggerThresholdBox
                        horizontalAlignment: Text.AlignRight
                        Layout.preferredWidth: 80
                        Layout.column: 1
                        Layout.row:2
                        inputMethodHints: Qt.ImhFormattedNumbersOnly

                        text: configModel.getValue("GamePad_Trigger_Threshold")

                        onTextEdited: {
                            configModel.setValue("GamePad_Trigger_Threshold", text)
                        }
                    }
                }
            }
        }

        RowLayout {
            Layout.fillWidth: true

            ColumnLayout {
                Layout.fillHeight: true
                Layout.fillWidth: true

                GroupBox {
                    Layout.fillHeight: true
                    Layout.fillWidth: true
                    title: qsTr("Left Joy Con")

                    ColumnLayout {
                        anchors.fill: parent

                        GroupBox {
                            Layout.fillHeight: true
                            Layout.fillWidth: true
                            title: qsTr("Stick")

                            GridLayout {
                                anchors.fill: parent
                                rows: 3
                                columns: 3

                                ColumnLayout {
                                    Layout.row: 0
                                    Layout.column: 1

                                    Label {
                                        text: "Stick"
                                    }

                                    Button {
                                        Layout.alignment: Qt.AlignHCenter | Qt.AlignVCenter
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: leftStick

                                        text: configModel.getValue("Controls_Left_JoyConController_Stick")

                                        onClicked: {
                                            var task = configModel.getGamePadInput
                                                    ("Controls_Left_JoyConController_Stick",
                                                     parseInt(gamePadIndexBox.text))

                                            Net.await(task, function(result)
                                            {
                                                if(result !== "")
                                                {
                                                    text = result
                                                }
                                            })
                                        }
                                    }
                                }

                                ColumnLayout {
                                    Layout.row: 1
                                    Layout.column: 1

                                    Label {
                                        text: "Button"
                                    }

                                    Button {
                                        Layout.alignment: Qt.AlignHCenter | Qt.AlignVCenter
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: leftStickButton

                                        text: configModel.getValue("Controls_Left_JoyConController_Stick_Button")

                                        onClicked: {
                                            var task = configModel.getGamePadInput
                                                    ("Controls_Left_JoyConController_Stick_Button",
                                                     parseInt(gamePadIndexBox.text))

                                            Net.await(task, function(result)
                                            {
                                                if(result !== "")
                                                {
                                                    text = result
                                                }
                                            })
                                        }
                                    }
                                }
                            }
                        }

                        GroupBox {
                            Layout.fillHeight: true
                            Layout.fillWidth: true
                            title: qsTr("D-Pad")

                            GridLayout {
                                anchors.fill: parent
                                rows: 3
                                columns: 3

                                ColumnLayout {
                                    Layout.fillHeight: true
                                    Layout.fillWidth: true
                                    Layout.row: 0
                                    Layout.column: 1

                                    Label {
                                        text: "Up"
                                    }

                                    Button {
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: leftDPadUpButton

                                        text: configModel.getValue("Controls_Left_JoyConController_DPad_Up")

                                        onClicked: {
                                            var task = configModel.getGamePadInput
                                                    ("Controls_Left_JoyConController_DPad_Up",
                                                     parseInt(gamePadIndexBox.text))

                                            Net.await(task, function(result)
                                            {
                                                if(result !== "")
                                                {
                                                    text = result
                                                }
                                            })
                                        }
                                    }
                                }

                                ColumnLayout {
                                    Layout.fillHeight: true
                                    Layout.fillWidth: true
                                    Layout.row: 2
                                    Layout.column: 1

                                    Label {
                                        text: "Down"
                                    }

                                    Button {
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: leftDPadDownButton

                                        text: configModel.getValue("Controls_Left_JoyConController_DPad_Down")

                                        onClicked: {
                                            var task = configModel.getGamePadInput
                                                    ("Controls_Left_JoyConController_DPad_Down",
                                                     parseInt(gamePadIndexBox.text))

                                            Net.await(task, function(result)
                                            {
                                                if(result !== "")
                                                {
                                                    text = result
                                                }
                                            })
                                        }
                                    }
                                }

                                ColumnLayout {
                                    Layout.fillHeight: true
                                    Layout.fillWidth: true
                                    Layout.row: 1
                                    Layout.column: 0

                                    Label {
                                        text: "Left"
                                    }

                                    Button {
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: leftDPadLeftButton

                                        text: configModel.getValue("Controls_Left_JoyConController_DPad_Left")

                                        onClicked: {
                                            var task = configModel.getGamePadInput
                                                    ("Controls_Left_JoyConController_DPad_Left",
                                                     parseInt(gamePadIndexBox.text))

                                            Net.await(task, function(result)
                                            {
                                                if(result !== "")
                                                {
                                                    text = result
                                                }
                                            })
                                        }
                                    }

                                }

                                ColumnLayout {
                                    Layout.fillHeight: true
                                    Layout.fillWidth: true
                                    Layout.row: 1
                                    Layout.column: 2

                                    Label {
                                        text: "Right"
                                    }

                                    Button {
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: leftDPadRightButton

                                        text: configModel.getValue("Controls_Left_JoyConController_DPad_Right")

                                        onClicked: {
                                            var task = configModel.getGamePadInput
                                                    ("Controls_Left_JoyConController_DPad_Right",
                                                     parseInt(gamePadIndexBox.text))

                                            Net.await(task, function(result)
                                            {
                                                if(result !== "")
                                                {
                                                    text = result
                                                }
                                            })
                                        }
                                    }
                                }
                            }
                        }

                        GroupBox {
                            Layout.fillHeight: true
                            Layout.fillWidth: true
                            title: qsTr("Buttons")

                            ColumnLayout {
                                anchors.fill: parent

                                RowLayout {
                                    Layout.fillHeight: true
                                    Layout.fillWidth: true

                                    ColumnLayout {
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        Layout.row: 2
                                        Layout.column: 1

                                        Label {
                                            text: "L"
                                        }

                                        Button {
                                            Layout.fillHeight: true
                                            Layout.fillWidth: true
                                            id: lButton

                                            text: configModel.getValue("Controls_Left_JoyConController_Button_L")

                                            onClicked: {
                                                var task = configModel.getGamePadInput
                                                        ("Controls_Left_JoyConController_Button_L",
                                                         parseInt(gamePadIndexBox.text))

                                                Net.await(task, function(result)
                                                {
                                                    if(result !== "")
                                                    {
                                                        text = result
                                                    }
                                                })
                                            }
                                        }
                                    }

                                    ColumnLayout {
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        Layout.row: 2
                                        Layout.column: 1

                                        Label {
                                            text: "ZL"
                                        }

                                        Button {
                                            Layout.fillHeight: true
                                            Layout.fillWidth: true
                                            id: zLButton

                                            text: configModel.getValue("Controls_Left_JoyConController_Button_ZL")

                                            onClicked: {
                                                var task = configModel.getGamePadInput
                                                        ("Controls_Left_JoyConController_Button_ZL",
                                                         parseInt(gamePadIndexBox.text))

                                                Net.await(task, function(result)
                                                {
                                                    if(result !== "")
                                                    {
                                                        text = result
                                                    }
                                                })
                                            }
                                        }
                                    }
                                }

                                ColumnLayout {
                                    Layout.fillHeight: true
                                    Layout.fillWidth: true
                                    Layout.row: 2
                                    Layout.column: 1

                                    Label {
                                        text: "-"
                                    }

                                    Button {
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: minusButton

                                        text: configModel.getValue("Controls_Left_JoyConController_Button_Minus")

                                        onClicked: {
                                            var task = configModel.getGamePadInput
                                                    ("Controls_Left_JoyConController_Button_Minus",
                                                     parseInt(gamePadIndexBox.text))

                                            Net.await(task, function(result)
                                            {
                                                if(result !== "")
                                                {
                                                    text = result
                                                }
                                            })
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ColumnLayout {

                GroupBox {
                    Layout.fillHeight: true
                    Layout.fillWidth: true
                    title: qsTr("Right Joy Con")

                    ColumnLayout {
                        anchors.fill: parent
                        GroupBox {
                            Layout.fillHeight: true
                            Layout.fillWidth: true
                            title: qsTr("Stick")

                            GridLayout {
                                anchors.fill: parent
                                rows: 3
                                columns: 3

                                ColumnLayout {
                                    Layout.row: 0
                                    Layout.column: 1

                                    Label {
                                        text: "Stick"
                                    }

                                    Button {
                                        Layout.alignment: Qt.AlignHCenter | Qt.AlignVCenter
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: rightStick

                                        text: configModel.getValue("Controls_Right_JoyConController_Stick")

                                        onClicked: {
                                            var task = configModel.getGamePadInput
                                                    ("Controls_Right_JoyConController_Stick",
                                                     parseInt(gamePadIndexBox.text))

                                            Net.await(task, function(result)
                                            {
                                                if(result !== "")
                                                {
                                                    text = result
                                                }
                                            })
                                        }
                                    }
                                }

                                ColumnLayout {
                                    Layout.fillHeight: true
                                    Layout.fillWidth: true
                                    Layout.row: 1
                                    Layout.column: 1

                                    Label {
                                        text: "Button"
                                    }

                                    Button {
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: rightStickButton

                                        text: configModel.getValue("Controls_Right_JoyConController_Stick_Button")

                                        onClicked: {
                                            var task = configModel.getGamePadInput
                                                    ("Controls_Right_JoyConController_Stick_Button",
                                                     parseInt(gamePadIndexBox.text))

                                            Net.await(task, function(result)
                                            {
                                                if(result !== "")
                                                {
                                                    text = result
                                                }
                                            })
                                        }
                                    }
                                }
                            }
                        }

                        GroupBox {
                            Layout.fillHeight: true
                            Layout.fillWidth: true
                            title: qsTr("Main Buttons")

                            GridLayout {
                                anchors.fill: parent
                                rows: 3
                                columns: 3

                                ColumnLayout {
                                    Layout.fillHeight: true
                                    Layout.fillWidth: true
                                    Layout.row: 0
                                    Layout.column: 1

                                    Label {
                                        text: "X"
                                    }

                                    Button {
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: xButton

                                        text: configModel.getValue("Controls_Right_JoyConController_Button_X")

                                        onClicked: {
                                            var task = configModel.getGamePadInput
                                                    ("Controls_Right_JoyConController_Button_X",
                                                     parseInt(gamePadIndexBox.text))

                                            Net.await(task, function(result)
                                            {
                                                if(result !== "")
                                                {
                                                    text = result
                                                }
                                            })
                                        }
                                    }
                                }

                                ColumnLayout {
                                    Layout.fillHeight: true
                                    Layout.fillWidth: true
                                    Layout.row: 2
                                    Layout.column: 1

                                    Label {
                                        text: "B"
                                    }

                                    Button {
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: bButton

                                        text: configModel.getValue("Controls_Right_JoyConController_Button_B")

                                        onClicked: {
                                            var task = configModel.getGamePadInput
                                                    ("Controls_Right_JoyConController_Button_B",
                                                     parseInt(gamePadIndexBox.text))

                                            Net.await(task, function(result)
                                            {
                                                if(result !== "")
                                                {
                                                    text = result
                                                }
                                            })
                                        }
                                    }
                                }

                                ColumnLayout {
                                    Layout.fillHeight: true
                                    Layout.fillWidth: true
                                    Layout.row: 1
                                    Layout.column: 0

                                    Label {
                                        text: "Y"
                                    }

                                    Button {
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: yButton

                                        text: configModel.getValue("Controls_Right_JoyConController_Button_Y")

                                        onClicked: {
                                            var task = configModel.getGamePadInput
                                                    ("Controls_Right_JoyConController_Button_Y",
                                                     parseInt(gamePadIndexBox.text))

                                            Net.await(task, function(result)
                                            {
                                                if(result !== "")
                                                {
                                                    text = result
                                                }
                                            })
                                        }
                                    }

                                }

                                ColumnLayout {
                                    Layout.fillHeight: true
                                    Layout.fillWidth: true
                                    Layout.row: 1
                                    Layout.column: 2

                                    Label {
                                        text: "A"
                                    }

                                    Button {
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: aButton

                                        text: configModel.getValue("Controls_Right_JoyConController_Button_A")

                                        onClicked: {
                                            var task = configModel.getGamePadInput
                                                    ("Controls_Right_JoyConController_Button_A",
                                                     parseInt(gamePadIndexBox.text))

                                            Net.await(task, function(result)
                                            {
                                                if(result !== "")
                                                {
                                                    text = result
                                                }
                                            })
                                        }
                                    }
                                }
                            }
                        }

                        GroupBox {
                            Layout.fillHeight: true
                            Layout.fillWidth: true
                            title: qsTr("Extra Buttons")

                            ColumnLayout {
                                anchors.fill: parent

                                RowLayout {
                                    Layout.fillHeight: true
                                    Layout.fillWidth: true

                                    ColumnLayout {
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        Layout.row: 2
                                        Layout.column: 1

                                        Label {
                                            text: "R"
                                        }

                                        Button {
                                            Layout.fillHeight: true
                                            Layout.fillWidth: true
                                            id: rButton

                                            text: configModel.getValue("Controls_Right_JoyConController_Button_R")

                                            onClicked: {
                                                var task = configModel.getGamePadInput
                                                        ("Controls_Right_JoyConController_Button_R",
                                                         parseInt(gamePadIndexBox.text))

                                                Net.await(task, function(result)
                                                {
                                                    if(result !== "")
                                                    {
                                                        text = result
                                                    }
                                                })
                                            }
                                        }
                                    }

                                    ColumnLayout {
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        Layout.row: 2
                                        Layout.column: 1

                                        Label {
                                            text: "ZR"
                                        }

                                        Button {
                                            Layout.fillHeight: true
                                            Layout.fillWidth: true
                                            id: zRButton

                                            text: configModel.getValue("Controls_Right_JoyConController_Button_ZR")

                                            onClicked: {
                                                var task = configModel.getGamePadInput
                                                        ("Controls_Right_JoyConController_Button_ZR",
                                                         parseInt(gamePadIndexBox.text))

                                                Net.await(task, function(result)
                                                {
                                                    if(result !== "")
                                                    {
                                                        text = result
                                                    }
                                                })
                                            }
                                        }
                                    }
                                }

                                ColumnLayout {
                                    Layout.fillHeight: true
                                    Layout.fillWidth: true
                                    Layout.row: 2
                                    Layout.column: 1

                                    Label {
                                        text: "+"
                                    }

                                    Button {
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: plusButton

                                        text: configModel.getValue("Controls_Right_JoyConController_Button_Plus")

                                        onClicked: {
                                            var task = configModel.getGamePadInput
                                                    ("Controls_Right_JoyConController_Button_Plus",
                                                     parseInt(gamePadIndexBox.text))

                                            Net.await(task, function(result)
                                            {
                                                if(result !== "")
                                                {
                                                    text = result
                                                }
                                            })
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    MessageDialog {
        id: inputWaitMessageDialog
        text: "Please press a key..."
        standardButtons: StandardButton.Close

        onRejected: {
            configModel.releaseWait()
        }
    }

    MessageDialog {
        id: errorAlert
        standardButtons: StandardButton.Close
        icon: StandardIcon.Critical

        onRejected: {
            configModel.releaseWait()
        }
    }

    ConfigurationModel {
        id: configModel

        onWaitReleased: {
            if(inputWaitMessageDialog.visible){
                inputWaitMessageDialog.close()
            }
        }

        onShowWaitDialog: {
            inputWaitMessageDialog.open()
        }

        onShowError: function(error,message) {
            errorAlert.text  = message
            errorAlert.title = error

            errorAlert.open()
        }
    }
}
