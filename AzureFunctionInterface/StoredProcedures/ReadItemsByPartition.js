function ReadItemsByPartition() {
    var collection = getContext().getCollection();
    var isAccepted = collection.queryDocuments(
        collection.getSelfLink(),
        'SELECT c.FirstName,c.LastName,c.Address,c.Phonenumber FROM c',
        function (err, feed) {
            if (err) throw err;
            if (!feed || !feed.length) {
                var response = getContext().getResponse();
                response.setBody('no docs found');
            }
            else {
                var response = getContext().getResponse();
                response.setBody(JSON.stringify(feed));
            }
        });

    if (!isAccepted) throw new Error('The query was not accepted by the server.');
}