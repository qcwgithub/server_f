function main() {
    let isDevelopment = (process.env.NODE_ENV === 'development');
    console.log('isDevelopment ? ' + (isDevelopment ? 'yes' : 'no'));

    let workingDir = process.cwd();
    console.log('workingDir: ' + workingDir);
}
main();