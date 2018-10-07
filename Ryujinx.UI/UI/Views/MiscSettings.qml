import QtQuick 2.0
import QtQuick.Controls 2.3
import QtQuick.Layouts 1.3

Frame {
    id: miscSettingsFrame
    width: 500
    height: 480
    Column{
        anchors.fill: parent

        GroupBox {
            anchors.fill: parent
            title: qsTr("Logging")

            Column {
                id: column
                anchors.fill: parent
                CheckBox {
                    id: loggingCheckBox
                    text: "Enable Logging"

                }

                Row {
                    id: row
                    anchors.top: loggingCheckBox.bottom
                    anchors.topMargin: 0
                    GroupBox {
                        id: logLevelsGroup
                        title: qsTr("Log Levels")

                        Column {
                            anchors.fill: parent

                            CheckBox {
                                id: debugLogCheckBox
                                text: "Debug"
                            }

                            CheckBox {
                                id: stubLogCheckBox
                                text: "Stub"
                            }

                            CheckBox {
                                id: infoLogCheckBox
                                text: "Info"
                            }

                            CheckBox {
                                id: warnLogCheckBox
                                text: "Warning"
                            }

                            CheckBox {
                                id: errorLogCheckBox
                                text: "Error"
                            }
                        }
                    }

                    GroupBox {
                        id: groupBox
                        anchors.left: logLevelsGroup.right
                        anchors.leftMargin: 0
                        title: qsTr("Log Classes")
                        width: Math.min(200, column.width - logLevelsGroup.width) - 20
                        height: logLevelsGroup.height

                        Column {
                            id: column1
                            anchors.fill: parent

                            TextArea {
                                id: logClassesTextArea
                                anchors.fill: parent

                            }
                        }
                    }

                }
            }
        }
    }
}

/*##^## Designer {
    D{i:29;anchors_height:-243;anchors_width:-111}D{i:4;anchors_height:0;anchors_width:0}
}
 ##^##*/
