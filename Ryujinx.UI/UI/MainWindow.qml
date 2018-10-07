import QtQuick 2.11
import QtQuick.Layouts 1.3
import QtQuick.Controls 2.4
import QtQuick.Controls.Material 2.4
import QtQuick.Dialogs 1.3

import Ryujinx 1.0

ApplicationWindow {
    id: window
    width: 840
    height: 680
    visible: true
    title: "Ryujinx"

    menuBar: MenuBar {
        Menu{
            leftPadding: 5
            leftMargin: 0
            title: "&File"


            MenuItem {
                id: loadGameMenuItem
                text: "Load Game File"
                onClicked: {
                    loadDialog.loadGame()
                }
            }

            MenuItem {
                id: loadGameFolderMenuItem
                text: "Load Game Folder"
                onClicked: {
                    loadDialog.loadGame()
                }
            }

            MenuItem {
                id: configMenuItem
                text: "Configuration"
                onClicked: {
                    var component = Qt.createComponent("./Views/Configuration.qml")
                    var configWindow    = component.createObject(window)
                    configWindow.show()
                }
            }

            MenuSeparator{}

            MenuItem {
                text: "Exit"
                onClicked: {
                    Qt.quit()
                }
            }
        }
    }

    header: ToolBar {
        id: toolBar

        RowLayout {
            id: rowLayout
            anchors.fill: parent
            spacing: 20

            RowLayout {
                id: mainControlPanel
                spacing: 20
                Layout.fillHeight: true
                Layout.fillWidth: true

                ToolButton {
                    id: openGameFileButton
                    text: qsTr("Load Game File")
                    display: AbstractButton.TextUnderIcon
                    icon.source: "./Images/loadGame.svg"
                    ToolTip {
                        text: qsTr("Load Game File")
                    }

                    onClicked: {
                        loadDialog.loadGame()
                    }
                }

                ToolButton {
                    id: openGameFolderButton
                    text: qsTr("Load Game Folder")
                    display: AbstractButton.TextUnderIcon
                    icon.source: "./Images/loadFolder.svg"
                    ToolTip {
                        text: qsTr("Load Game Folder")
                    }

                    onClicked: {
                        loadDialog.loadGameFolder()
                    }
                }

                ToolSeparator{}

                ToolButton {
                    id: closeGameButton
                    text: qsTr("Stop")
                    display: AbstractButton.TextUnderIcon
                    icon.source: "./Images/closeGame.svg"
                    enabled: false

                    ToolTip {
                        text: qsTr("Close Current Game")
                    }

                    onClicked: {
                        controller.shutdownEmulation()
                    }
                }
            }
        }
    }

    StackView {
        id: contentStack
        anchors.fill: parent
    }

    FileDialog {
        id: loadDialog
        selectMultiple: false
        nameFilters: ["Game Carts (*.xci)",
            "Application Packages (*.nca *.nsp)",
            "Executable (*.nso *.nro)",
            "All Supported Formats (*.xci *.nca *.nsp *.nso *.nro)"]
        folder: shortcuts.home

        onAccepted: {
            if(selectFolder )
            {
                controller.loadGameFolder(fileUrl)
            }
            else
            {
                controller.loadGameFile(fileUrl)
            }
        }

        function loadGame() {
            selectFolder = false
            title        = qsTr("Load Game File")

            open()
        }

        function loadGameFolder() {
            selectFolder = true
            title        = qsTr("Load Game Folder")

            open()
        }
    }



    EmulationController {
        id: controller

        onFailed: function(result) {
           // alertBox.title = "Failed to load game"
           // alertBox.text  = result

           // alertBox.open()

            loadGameMenuItem.enabled       = true
            loadGameFolderMenuItem.enabled = true
            openGameFileButton.enabled     = true
            openGameFolderButton.enabled   = true
            closeGameButton.enabled        = false
        }

        onSuccess: {
            loadGameMenuItem.enabled       = false
            loadGameFolderMenuItem.enabled = false
            openGameFileButton.enabled     = false
            openGameFolderButton.enabled   = false
            closeGameButton.enabled        = true
        }

        onLoaded: {
            loadGameMenuItem.enabled       = false
            loadGameFolderMenuItem.enabled = false
            openGameFileButton.enabled     = false
            openGameFolderButton.enabled   = false
            closeGameButton.enabled        = true
        }

        onUnloaded: {
            loadGameMenuItem.enabled       = true
            loadGameFolderMenuItem.enabled = true
            openGameFileButton.enabled     = true
            openGameFolderButton.enabled   = true
            closeGameButton.enabled        = false
        }
    }
}
