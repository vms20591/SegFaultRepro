function asyncTask(ms) {
    return new Promise((res, rej) => {
        if (ms > 0) {
            delay(ms);
            res();
            return;
        }

        rej("Delay should be greater than 0");
    });
}

function delay(ms) {
    let last = (new Date()).getTime();
    while (true) {
        let now = (new Date()).getTime();
        if (now - last >= ms) {
            return;
        }
    }
}

runtime.echo('Started execution @ ' + new Date().toISOString());

asyncTask(100).then(() => {
    runtime.echo('asyncTask done @ ' + new Date().toISOString())
}).catch(err => {
    runtime.echo('asyncTask done with error ' + err + ' @ ' + new Date().toISOString());
}); 