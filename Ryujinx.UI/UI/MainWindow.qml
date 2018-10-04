import QtQuick 2.9
import QtQuick.Layouts 1.3
import QtQuick.Controls 2.3
import QtQuick.Controls.Material 2.1
import QtQuick.Dialogs 1.0

ApplicationWindow {
    id: window
    width: 840
    height: 680
    visible: true
    title: "Ryujinx"

    Material.theme: Material.Light
    Material.accent: '#41cd52'
    Material.primary: '#41cd52'

    header: ToolBar {
        id: toolBar

        RowLayout {
            id: rowLayout
            anchors.fill: parent
            spacing: 20

            ToolButton {
                id: drawerButton
                text: qsTr("")
                spacing: 3
                display: AbstractButton.IconOnly
                icon.source: "./Images/drawer.png"

                onClicked: {
                    if (contentStack.depth > 1) {
                        contentStack.pop()
                    } else {
                        drawer.open()
                    }
                }
            }

            RowLayout {
                id: mainControlPanel
                Layout.fillHeight: true
                Layout.fillWidth: true
                Layout.alignment: Qt.AlignHCenter | Qt.AlignVCenter

                ToolButton {
                    id: openGameFileButton
                    display: AbstractButton.IconOnly
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
                    display: AbstractButton.IconOnly
                    icon.source: "./Images/loadFolder.svg"
                    ToolTip {
                        text: qsTr("Load Game Folder")
                    }

                    onClicked: {
                    loadDialog.loadGameFolder()
                    }
                }
            }

            ToolButton {
                id: menuButton
                text: qsTr("Tool Button")
            }
        }
    }

    StackView {
        id: contentStack
        anchors.fill: parent
    }

    Drawer {
            id: drawer
            width: Math.min(window.width, window.height) / 3 * 2
            height: window.height
            interactive: stackView.depth === 1

            ListView {
                id: drawerMenuList
                focus: true
                currentIndex: -1
                anchors.fill: parent

                delegate: ItemDelegate {
                    width: parent.width
                    text: model.title
                    highlighted: ListView.isCurrentItem
                }

                model: ListModel {
                    ListElement { title: "Games"}
                    ListElement { title: "Settings"}
                    ListElement { title: "Exit"}
                }

                ScrollIndicator.vertical: ScrollIndicator { }
            }
    }

    FileDialog {
        id: loadDialog
        selectMultiple: false
        nameFilters: ["Game Carts (*.xci)",
                      "Application Packages (*.nca *.nsp)",
                      "Executable (*.nso *.nro)",
                      "All Supported Formats (*.xci *.nca *.nsp *.nso *.nro)"]

        Component.onCompleted: visible = false

        function loadGame() {
            selectFolder = false
            title        = qsTr("Load Game File")

            show()
        }

        function loadGameFolder() {
            selectFolder = true
            title        = qsTr("Load Game Folder")

            show()
        }
    }
}
