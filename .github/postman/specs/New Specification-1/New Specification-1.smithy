$version: "2"

namespace com.example

use aws.protocols#restJson1

/// A simple greeting service
@restJson1
service GreetingService {
    version: "2024-01-01"
    operations: [GetGreeting, CreateGreeting]
    errors: [ValidationError]
}

/// Returns a greeting for the given name
@http(method: "GET", uri: "/greeting/{name}", code: 200)
@readonly
operation GetGreeting {
    input := {
        /// The name to greet
        @required
        @httpLabel
        name: String
    }
    output := {
        /// The greeting message
        @required
        message: String
    }
    errors: [GreetingNotFoundError]
}

/// Creates a new greeting
@http(method: "POST", uri: "/greeting", code: 201)
operation CreateGreeting {
    input := {
        /// The name to greet
        @required
        name: String

        /// Optional custom message prefix
        prefix: String = "Hello"
    }
    output := {
        /// The created greeting message
        @required
        message: String
    }
}

/// Thrown when a greeting is not found
@error("client")
@httpError(404)
structure GreetingNotFoundError {
    @required
    message: String
}

/// Thrown when the request input is invalid
@error("client")
@httpError(400)
structure ValidationError {
    @required
    message: String

    /// Field-level validation errors
    fieldErrors: FieldErrorMap
}

map FieldErrorMap {
    key: String
    value: String
}
