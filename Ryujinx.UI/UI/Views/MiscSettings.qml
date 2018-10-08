import QtQuick 2.0
import QtQuick.Controls 2.3
import QtQuick.Layouts 1.3

Frame {
    id: miscSettingsFrame
    width: 500
    height: 480
    ColumnLayout {
        anchors.fill: parent

        GroupBox {
            Layout.alignment: Qt.AlignLeft | Qt.AlignTop
            Layout.fillHeight: false
            Layout.fillWidth: true
            title: qsTr("Logging")

            ColumnLayout {
                id: column
                anchors.fill: parent
                CheckBox {
                    id: loggingCheckBox
                    text: "Enable Logging"
                    Layout.fillHeight: false
                    Layout.fillWidth: true

                }

                RowLayout {
                    id: row
                    Layout.alignment: Qt.AlignLeft | Qt.AlignTop
                    Layout.fillHeight: false
                    Layout.fillWidth: true

                    GroupBox {
                        id: logLevelsGroup
                        Layout.fillHeight: true
                        Layout.alignment: Qt.AlignLeft | Qt.AlignTop
                        Layout.fillWidth: false
                        title: qsTr("Log Levels")

                        ColumnLayout {
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
                        title: qsTr("Log Classes")
                        Layout.fillHeight: true
                        Layout.fillWidth: true

                        TextEdit {
                            id: logClassesTextArea
                            selectionColor: "#004a80"
                            renderType: Text.NativeRendering
                            anchors.fill: parent

                        }
                    }

                }
            }
        }
    }
}
