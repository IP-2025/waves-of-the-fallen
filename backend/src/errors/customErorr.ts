export class CustomError extends Error {
  status: number;

  constructor(message: string, status: number) {
    super(message);
    this.status = status;
    // Set the prototype explicitly to maintain the instance of CustomError
    Object.setPrototypeOf(this, CustomError.prototype);
  }
}
