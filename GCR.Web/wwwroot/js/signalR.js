
const connection = new signalR
	.HubConnectionBuilder()
	.withUrl("/HomeHub")
	.build();

connection.serverTimeoutInMilliseconds = 24000;
connection.keepAliveIntervalInMilliseconds = 12000;

window.hub = {
	evs: [],
	on(eName, func) {
		if (!this.evs[eName]) {// 判断事件是否存在
			this.evs[eName] = [];
		}
		this.evs[eName].push(func);
	},
	off(eName, func) {
		if (!func) {
			this.evs[eName] = [];
		} else {
			var arr = this.evs[eName];
			this.evs[eName].splice(arr.findIndex(item => item === func), 1);
		}
	},
	emit(eName, params) {
		this.evs[eName] && this.evs[eName].forEach(function (func) {
			try {
				func && func.call(this, params);
			}
			catch (ex) { }
		})
	},
	//向全局发送数据
	invoke(name, msg, to = "") {
		msg = { evname: name, msg };
		connection.invoke("send", JSON.stringify(msg), to);
	},
	//向本地全局发送数据
	invokeSelf(name, msg) {
		this.invoke(name, msg, this.id);
	}
};
//链接后返回
connection.on("conncted", (id) => {
	console.log(id);
	window.hub.id = id;
	F.cookie('hubid', id);
});
connection.on("HubMessage", (opt) => {
	opt = JSON.parse(opt);
	window.hub.emit(opt.evname, opt.msg);
});
connection.on("ExecuteScript", (fun) => {
	if (F.isSTR(fun)) {
		new Function(fun)();
	} else if (F.isFUN(fun)) {
		fun.apply();
	}
});
connection.on("ExecuteScriptAsync", async (fun) => {
	if (F.isSTR(fun)) {
		await new Function('return ' + fun)();
	} else if (F.isFUN(fun)) {
		await fun.apply();
	}
});
//链接
const start = () => {
	connection.start().then(() => {
		console.log('SR链接成功>');
		window.hub.emit("connection", connection);
	}).catch(err => {
		setTimeout(() => start(), 1000);
		window.hub.emit("connectionclose", connection);
		return console.error(err.toString());
	});
};
connection.onclose(() => {
	console.log('SR断开>');
	window.hub.emit("connectionclose", connection);
	start();
});
start();
window.hub.connection = connection;