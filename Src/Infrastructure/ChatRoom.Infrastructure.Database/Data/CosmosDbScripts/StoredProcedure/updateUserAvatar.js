function updateUserAvatar(newAvatar) {
    var container = getContext().getCollection();
    var response = getContext().getResponse();

    var isAccepted = container.queryDocuments(
        container.getSelfLink(),
        'SELECT * FROM message',

        function (err, messages) {
            if (err) throw new Error(err.message);

            if (!messages || !messages.length) {
                response.setBody('no docs found');
            } else {
                updateContainerUserAvatar(messages);
            }
        }
    );

    function updateContainerUserAvatar(messages) {

        for (let i = 0; i < messages.length; i++) {
            messages[i].user.avatar = newAvatar;
            container.replaceDocument(messages[i]._self, messages[i], function (err) {
                if (err) throw Error(err.message);
            })
        }

        response.setBody("OK");
    }

    if (!isAccepted) throw new Error('The query was not accepted by the server.');
}