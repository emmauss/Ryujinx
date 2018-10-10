import QtQuick 2.0
import QtQuick.Controls 2.3
import QtQuick.Layouts 1.3
import QtQuick.Dialogs 1.3
import Ryujinx 1.0

Frame {
    id: keyboardSettingsFrame
    width: 800
    height: 600

    RowLayout {
        anchors.fill: parent


        ColumnLayout {

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
                                    id: leftStickUpButton

                                    text: configModel.getKeyboardKey("Controls_Left_JoyConKeyboard_Stick_Up")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Left_JoyConKeyboard_Stick_Up")
                                        Net.await(task, function(result){
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
                                    id: leftStickDownButton

                                    text: configModel.getKeyboardKey("Controls_Left_JoyConKeyboard_Stick_Down")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Left_JoyConKeyboard_Stick_Down")
                                        Net.await(task, function(result){
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
                                    id: leftStickLeftButton

                                    text: configModel.getKeyboardKey("Controls_Left_JoyConKeyboard_Stick_Left")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Left_JoyConKeyboard_Stick_Left")
                                        Net.await(task, function(result){
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
                                    id: leftStickRightButton

                                    text: configModel.getKeyboardKey("Controls_Left_JoyConKeyboard_Stick_Right")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Left_JoyConKeyboard_Stick_Right")
                                        Net.await(task, function(result){
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
                                    id: leftStickButton

                                    text: configModel.getKeyboardKey("Controls_Left_JoyConKeyboard_Stick_Button")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Left_JoyConKeyboard_Stick_Button")
                                        Net.await(task, function(result){
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

                                    text: configModel.getKeyboardKey("Controls_Left_JoyConKeyboard_DPad_Up")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Left_JoyConKeyboard_DPad_Up")
                                        Net.await(task, function(result){
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

                                    text: configModel.getKeyboardKey("Controls_Left_JoyConKeyboard_DPad_Down")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Left_JoyConKeyboard_DPad_Down")
                                        Net.await(task, function(result){
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

                                    text: configModel.getKeyboardKey("Controls_Left_JoyConKeyboard_DPad_Left")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Left_JoyConKeyboard_DPad_Left")
                                        Net.await(task, function(result){
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

                                    text: configModel.getKeyboardKey("Controls_Left_JoyConKeyboard_DPad_Right")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Left_JoyConKeyboard_DPad_Right")
                                        Net.await(task, function(result){
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

                                        text: configModel.getKeyboardKey("Controls_Left_JoyConKeyboard_Button_L")

                                        onClicked: {
                                            var task = configModel.getKeyboardInput
                                                    ("Controls_Left_JoyConKeyboard_Button_L")
                                            Net.await(task, function(result){
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

                                        text: configModel.getKeyboardKey("Controls_Left_JoyConKeyboard_Button_ZL")

                                        onClicked: {
                                            var task = configModel.getKeyboardInput
                                                    ("Controls_Left_JoyConKeyboard_Button_ZL")
                                            Net.await(task, function(result){
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

                                    text: configModel.getKeyboardKey("Controls_Left_JoyConKeyboard_Button_Minus")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Left_JoyConKeyboard_Button_Minus")
                                        Net.await(task, function(result){
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
                                    id: rightStickUpButton

                                    text: configModel.getKeyboardKey("Controls_Right_JoyConKeyboard_Stick_Up")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Right_JoyConKeyboard_Stick_Up")
                                        Net.await(task, function(result){
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
                                    id: rightStickDownButton

                                    text: configModel.getKeyboardKey("Controls_Right_JoyConKeyboard_Stick_Down")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Right_JoyConKeyboard_Stick_Down")
                                        Net.await(task, function(result){
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
                                    id: rightStickLeftButton

                                    text: configModel.getKeyboardKey("Controls_Right_JoyConKeyboard_Stick_Left")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Right_JoyConKeyboard_Stick_Left")
                                        Net.await(task, function(result){
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
                                    id: rightStickRightButton

                                    text: configModel.getKeyboardKey("Controls_Right_JoyConKeyboard_Stick_Right")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Right_JoyConKeyboard_Stick_Right")
                                        Net.await(task, function(result){
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

                                    text: configModel.getKeyboardKey("Controls_Right_JoyConKeyboard_Stick_Button")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Right_JoyConKeyboard_Stick_Button")
                                        Net.await(task, function(result){
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

                                    text: configModel.getKeyboardKey("Controls_Right_JoyConKeyboard_Button_X")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Right_JoyConKeyboard_Button_X")
                                        Net.await(task, function(result){
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

                                    text: configModel.getKeyboardKey("Controls_Right_JoyConKeyboard_Button_B")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Right_JoyConKeyboard_Button_B")
                                        Net.await(task, function(result){
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

                                    text: configModel.getKeyboardKey("Controls_Right_JoyConKeyboard_Button_Y")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Right_JoyConKeyboard_Button_Y")
                                        Net.await(task, function(result){
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

                                    text: configModel.getKeyboardKey("Controls_Right_JoyConKeyboard_Button_A")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Right_JoyConKeyboard_Button_A")
                                        Net.await(task, function(result){
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

                                        text: configModel.getKeyboardKey("Controls_Right_JoyConKeyboard_Button_R")

                                        onClicked: {
                                            var task = configModel.getKeyboardInput
                                                    ("Controls_Right_JoyConKeyboard_Button_R")
                                            Net.await(task, function(result){
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

                                        text: configModel.getKeyboardKey("Controls_Right_JoyConKeyboard_Button_ZR")

                                        onClicked: {
                                            var task = configModel.getKeyboardInput
                                                    ("Controls_Right_JoyConKeyboard_Button_ZR")
                                            Net.await(task, function(result){
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

                                    text: configModel.getKeyboardKey("Controls_Right_JoyConKeyboard_Button_Plus")

                                    onClicked: {
                                        var task = configModel.getKeyboardInput
                                                ("Controls_Right_JoyConKeyboard_Button_Plus")
                                        Net.await(task, function(result){
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

    MessageDialog {
        id: inputWaitMessageDialog
        text: "Please press a key..."
        standardButtons: StandardButton.Close

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
    }
}
