# POC

## Part of concept is to use mTLS 
 ### * Using trusted CA ( add custom attribute)
     * Device are provided to  mtls using x509 Certificate
     * Device send request to Server CA (Certificate and sn) -> get status 
     * Server get  response and drop the handshake to TLS
     * Signed HMAC-SHA256 message tampering
 ### * Store key and rotate regularly
      * keys managed via Azure Key Vault or local HSM
 ### * IdentityServer to Authorize or assign rule 
    Zero-trust -- every request is verified

 ### Device-software module we can build and PublishSingleFile using tools like: signtool.exe
     timestamp and can verify signature 
     we can sign it with 
 ### For IoT HMAC keys from manufacturing(specially when are custom) will be the best option