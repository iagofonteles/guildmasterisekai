namespace Drafts.Web {

    public enum RequestResult {
        //
        // Summary:
        //     The request hasn't finished yet.
        InProgress = 0,
        //
        // Summary:
        //     The request succeeded.
        Success = 1,
        //
        // Summary:
        //     Failed to communicate with the server. For example, the request couldn't connect
        //     or it could not establish a secure channel.
        ConnectionError = 2,
        //
        // Summary:
        //     The server returned an error response. The request succeeded in communicating
        //     with the server, but received an error as defined by the connection protocol.
        ProtocolError = 3,
        //
        // Summary:
        //     Error processing data. The request succeeded in communicating with the server,
        //     but encountered an error when processing the received data. For example, the
        //     data was corrupted or not in the correct format.
        DataProcessingError = 4
    }

}
