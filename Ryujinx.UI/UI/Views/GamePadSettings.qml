import QtQuick 2.0
import QtQuick.Controls 2.3
import QtQuick.Layouts 1.3

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
                }

                GridLayout {
                    Layout.fillHeight: true
                    Layout.fillWidth: true
                    columns: 2
                    rows: 3

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
                        Layout.fillWidth: true
                        Layout.column: 1
                        Layout.row:0
                        inputMethodHints: Qt.ImhDigitsOnly
                    }

                    TextField {
                        id: deadzoneBox
                        Layout.column: 1
                        Layout.row:1
                        inputMethodHints: Qt.ImhFormattedNumbersOnly
                    }

                    TextField {
                        id: triggerThresholdBox
                        Layout.column: 1
                        Layout.row:2
                        inputMethodHints: Qt.ImhFormattedNumbersOnly
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
                                    Layout.fillHeight: true
                                    Layout.fillWidth: true
                                    Layout.row: 0
                                    Layout.column: 1

                                    Label {
                                        text: "Up"
                                    }

                                    Button {
                                        text: "Up"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: leftStickUpButton
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
                                        text: "Down"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: leftStickDownButton
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
                                        text: "Left"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: leftStickLeftButton
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
                                        text: "Right"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: leftStickRightButton
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
                                        text: "Button"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: leftStickButton
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
                                        text: "Up"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: leftDPadUpButton
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
                                        text: "Down"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: leftDPadDownButton
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
                                        text: "Left"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: leftDPadLeftButton
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
                                        text: "Right"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: leftDPadRightButton
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
                                            text: "L"
                                            Layout.fillHeight: true
                                            Layout.fillWidth: true
                                            id: lButton
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
                                            text: "ZL"
                                            Layout.fillHeight: true
                                            Layout.fillWidth: true
                                            id: zLButton
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
                                        text: "-"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: minusButton
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
                                        text: "Up"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: rightStickUpButton
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
                                        text: "Down"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: rightStickDownButton
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
                                        text: "Left"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: rightStickLeftButton
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
                                        text: "Right"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: rightStickRightButton
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
                                        text: "Button"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: rightStickButton
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
                                        text: "X"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: xButton
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
                                        text: "B"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: bButton
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
                                        text: "Y"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: yButton
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
                                        text: "A"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: aButton
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
                                            text: "L"
                                            Layout.fillHeight: true
                                            Layout.fillWidth: true
                                            id: rButton
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
                                            text: "ZL"
                                            Layout.fillHeight: true
                                            Layout.fillWidth: true
                                            id: zRButton
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
                                        text: "+"
                                        Layout.fillHeight: true
                                        Layout.fillWidth: true
                                        id: plusButton
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
