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
                icon.source: !drawer.visible ? "./Images/drawer.png"
                                             : "./Images/arrowBack.svg"

                onClicked: {
                    if (drawer.visible) {
                        drawer.close()
                    } else {
                        drawer.open()
                    }
                }
            }

            RowLayout {
                id: mainControlPanel
                spacing: 20
                Layout.fillHeight: true
                Layout.fillWidth: true

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
        }
    }

    StackView {
        id: contentStack
        anchors.fill: parent
    }

    Drawer {
        id: drawer
        width: window.width / 3
        height: window.height
        topMargin: toolBar.height
        spacing: 10

        Rectangle{

            Column{
                id: column
                x: 40
                y: 20
                anchors.left: parent.left
                anchors.leftMargin: 40
                anchors.top: parent.top
                anchors.topMargin: 20
                spacing: 20

                Image {
                    id: logo
                    width: 100
                    height: 100
                    fillMode: Image.PreserveAspectFit
                    source: "./Images/ryujinxLogo.png"
                }

                Label {
                    id: appLabel
                    text: qsTr("Ryujinx")
                    font.bold: true
                    font.pointSize: 16
                    font.weight: Font.Bold
                    lineHeight: 1.2
                }

                Rectangle{
                    id: rectangle
                    anchors.top: appLabel.bottom
                    anchors.topMargin: 20

                    ListView {
                        id: drawerMenuList
                        width: 100
                        height: 120
                        anchors.top: parent.top
                        anchors.topMargin: 0
                        currentIndex: -1

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

            }

        }
    }

    FileDialog {
        id: loadDialog
        selectMultiple: false
        nameFilters: ["Game Carts (*.xci)",
            "Application Packages (*.nca *.nsp)",
            "Executable (*.nso *.nro)",
            "All Supported Formats (*.xci *.nca *.nsp *.nso *.nro)"]
        folder: shortcuts.home

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
}

/*##^## Designer {
    D{i:272;anchors_height:120;anchors_width:100}
}
 ##^##*/
