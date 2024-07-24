
// 获取 Pako 压缩的代码
function GetPako(code) {
    return serialize(JSON.stringify({
        code,
        mermaid: { theme: "default" },
        autoSync: true,
        updateDiagram: true,
        editorMode: "code"
    }));
}

// 序列化和反序列化函数
function serialize(state) {
    const data = new TextEncoder().encode(state);
    const compressed = pako.deflate(data, { level: 9 });
    return Base64.fromUint8Array(compressed, true);
}

function deserialize(state) {
    const data = Base64.toUint8Array(state);
    return pako.inflate(data, { to: 'string' });
}
