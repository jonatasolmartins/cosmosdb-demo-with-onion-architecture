function updateMessage(chatId, newMessage)
{
    var container = getContext().getCollection();
    var response = getContext().getResponse();

    var isAccepted = container.queryDocuments(
        container.getSelfLink(),
        'SELECT * FROM room',

        function(err, room) {
            if(err)throw new Error(err.message);

            if(!room || !room.length) {
                response.setBody('no docs found');
            }
            else{
                updateMessageOnChatDocument(room)
            }
        }

    );
    
    function updateMessageOnChatDocument(room)
    {
        let chat = room[0].chats.filter(chat => chat.id == chatId);

        if(chat[0].messages.length > 2)
        {
            chat[0].messages.shift();
        }
        
        chat[0].messages.push(newMessage);

        container.replaceDocument(room[0]._self, room[0],function (err){
            if(err) throw Error(err.message);
        })

        response.setBody(JSON.stringify(room[0]));
    }

    if (!isAccepted) throw new Error('The query was not accepted by the server.');
}
