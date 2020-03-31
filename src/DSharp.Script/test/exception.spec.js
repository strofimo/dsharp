
describe('exception', () => {
    test('exception is error type', () => {
        //Arrange
        const exception = require(process.env['RUNTIME']).Exception;

        //Act
        const ex = new exception("error message");

        //Assert
        expect(ex instanceof Error).toBe(true);
    });

    test('exception has message', () => {
        //Arrange
        const exception = require(process.env['RUNTIME']).Exception;

        //Act
        const ex = new exception("error message");

        //Assert
        expect(ex.message).toBe("error message");
    });

    test('exception has stack', () => {
        //Arrange
        const exception = require(process.env['RUNTIME']).Exception;

        //Act
        const ex = new exception("error message");

        //Assert
        expect(ex.stack).toContain("Exception: error message\n    at");
    });
});