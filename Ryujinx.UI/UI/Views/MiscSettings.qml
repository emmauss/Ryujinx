import QtQuick 2.0
import QtQuick.Controls 2.3
import QtQuick.Layouts 1.3
import Ryujinx 1.0

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

                                checked: configModel.getValue("Logging_Enable_Debug")

                                onCheckedChanged: {
                                    configModel.setValue("Logging_Enable_Debug", checked)
                                }
                            }

                            CheckBox {
                                id: stubLogCheckBox
                                text: "Stub"

                                checked: configModel.getValue("Logging_Enable_Stub")

                                onCheckedChanged: {
                                    configModel.setValue("Logging_Enable_Stub", checked)
                                }
                            }

                            CheckBox {
                                id: infoLogCheckBox
                                text: "Info"

                                checked: configModel.getValue("Logging_Enable_Info")

                                onCheckedChanged: {
                                    configModel.setValue("Logging_Enable_Info", checked)
                                }
                            }

                            CheckBox {
                                id: warnLogCheckBox
                                text: "Warning"

                                checked: configModel.getValue("Logging_Enable_Warn")

                                onCheckedChanged: {
                                    configModel.setValue("Logging_Enable_Warn", checked)
                                }
                            }

                            CheckBox {
                                id: errorLogCheckBox
                                text: "Error"

                                checked: configModel.getValue("Logging_Enable_Error")

                                onCheckedChanged: {
                                    configModel.setValue("Logging_Enable_Error", checked)
                                }
                            }
                        }
                    }

                    GroupBox {
                        id: groupBox
                        title: qsTr("Log Classes")
                        Layout.fillHeight: true
                        Layout.fillWidth: true

                        TextArea {
                            id: logClassesTextArea
                            selectionColor: "#004a80"
                            renderType: Text.NativeRendering
                            anchors.fill: parent
                            background: Rectangle {
                                color: "#ffffff"
                            }

                            text: configModel.getValue("Logging_Filtered_Classes")

                            onEditingFinished: {
                                configModel.setValue("Logging_Filtered_Classes", text)
                            }

                        }
                    }

                }
            }
        }
    }

    ConfigurationModel {
        id: configModel
    }
}
