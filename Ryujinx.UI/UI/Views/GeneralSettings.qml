import QtQuick 2.0
import QtQuick.Controls 2.3
import QtQuick.Layouts 1.3
import Ryujinx 1.0

Frame {
    id: generalSetttingsFrame

    Column {
        anchors.fill: parent

        GroupBox {
            title: qsTr("Game")

            Column {
                CheckBox {
                    id: dockedCheckBox
                    text: "Enable Docked Mode"

                    checked: configModel.getValue("Docked_Mode")

                    onCheckedChanged: {
                        configModel.setValue("Docked_Mode", checked)
                    }

                }

                CheckBox {
                    id: vsyncCheckBox
                    text: "Enable Ingame VSync"

                    checked: configModel.getValue("Enable_Vsync")

                    onCheckedChanged: {
                        configModel.setValue("Enable_Vsync", checked)
                    }

                }
            }
        }

        GroupBox {
            title: qsTr("System")

            Column {
                CheckBox {
                    id: memoryCheckBox
                    text: "Enable Memory Checks (slow)"

                    checked: configModel.getValue("Enable_Memory_Checks")

                    onCheckedChanged: {
                        configModel.setValue("Enable_Memory_Checks", checked)
                    }

                }

                CheckBox {
                    id: multiCoreCheckBox
                    text: "Enable MultiCore Scheduling"

                    checked: configModel.getValue("Enable_MultiCore_Scheduling")

                    onCheckedChanged: {
                        configModel.setValue("Enable_MultiCore_Scheduling", checked)
                    }

                }

                CheckBox {
                    id: fsIntegrityCheckBox
                    text: "Enable RomFS Integrity Checks"

                    checked: configModel.getValue("Enable_FS_Integrity_Checks")

                    onCheckedChanged: {
                        configModel.setValue("Enable_FS_Integrity_Checks", checked)
                    }

                }
            }
        }
    }

    ConfigurationModel {
        id:configModel
    }

}
