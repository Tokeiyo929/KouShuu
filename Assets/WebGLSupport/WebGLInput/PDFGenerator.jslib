mergeInto(LibraryManager.library, {
    // 存储jsPDF对象的全局变量
    _jsPDFLibrary: null,
    
    // 接收jsPDF对象的函数
    SetJSPDF: function(jsPDFObject) {
        try {
            console.log('SetJSPDF被调用，参数:', jsPDFObject);
            console.log('参数类型:', typeof jsPDFObject);
            
            // 将jsPDF对象存储到全局变量中
            this._jsPDFLibrary = jsPDFObject;
            console.log('jsPDF库已成功传递到.jslib环境:', typeof this._jsPDFLibrary);
            
            // 验证jsPDF对象是否可用
            if (this._jsPDFLibrary && typeof this._jsPDFLibrary === 'function') {
                console.log('jsPDF库验证成功，可以正常使用');
                console.log('jsPDF库构造函数:', this._jsPDFLibrary);
                return true;
            } else {
                console.error('jsPDF库对象无效，类型:', typeof this._jsPDFLibrary);
                console.error('jsPDF库对象值:', this._jsPDFLibrary);
                return false;
            }
        } catch (error) {
            console.error('设置jsPDF库时发生错误:', error);
            console.error('错误堆栈:', error.stack);
            return false;
        }
    },
    
    // 生成PDF的主函数
    GeneratePDF: function(jsonData) {
        try {
            // 优先使用已注入的jsPDF；否则尝试从window捕获（打破作用域隔离）
            var jsPDFRef = this._jsPDFLibrary
                || (typeof window !== 'undefined' && (window.jsPDF || (window.jspdf && window.jspdf.jsPDF)))
                || null;

            if (!jsPDFRef) {
                console.log('jsPDF库未设置，尝试加载本地jsPDF库...');
                
                if (typeof window !== 'undefined') {
                    // 尝试加载本地jsPDF库
                    var script = document.createElement('script');
                    script.src = 'jspdf.umd.min.js';
                    script.onload = function() {
                        console.log('本地jsPDF库加载成功');
                        if (window.jspdf && window.jspdf.jsPDF) {
                            window.jsPDF = window.jspdf.jsPDF;
                            console.log('jsPDF库已设置，重新尝试生成PDF');
                            setTimeout(function() {
                                _GeneratePDF(jsonData);
                            }, 500);
                        }
                    };
                    script.onerror = function() {
                        console.error('本地jsPDF库加载失败');
                        alert('PDF生成失败：无法加载jsPDF库，请确保jspdf.umd.min.js文件存在');
                    };
                    document.head.appendChild(script);
                    return;
                }
                
                console.error('jsPDF库未设置且无法从window捕获');
                alert('PDF生成失败：未找到jsPDF库，请刷新页面重试');
                return;
            }

            // 将可用引用缓存起来，后续直接使用
            this._jsPDFLibrary = jsPDFRef;
            
            var data = UTF8ToString(jsonData);
            console.log('接收到的原始数据:', data);
            
            // 清理数据，移除可能的无效字符
            var cleanData = data.replace(/[^\x20-\x7E\u4E00-\u9FFF\u3000-\u303F\uFF00-\uFFEF]/g, '');
            console.log('清理后的数据:', cleanData);
            
            var pdfData;
            try {
                pdfData = JSON.parse(cleanData);
            } catch (parseError) {
                console.error('JSON解析失败:', parseError);
                console.error('原始数据:', data);
                console.error('清理后数据:', cleanData);
                
                // 尝试更激进的清理
                var aggressiveCleanData = data.replace(/[^\x20-\x7E]/g, '');
                console.log('激进清理后的数据:', aggressiveCleanData);
                
                try {
                    pdfData = JSON.parse(aggressiveCleanData);
                    console.log('激进清理后JSON解析成功');
                } catch (secondError) {
                    console.error('激进清理后JSON仍然解析失败:', secondError);
                    alert('PDF生成失败：数据格式错误，请检查Unity传递的数据');
                    return;
                }
            }
            
            console.log('开始生成PDF，数据：', pdfData);
            console.log('使用的jsPDF库:', typeof this._jsPDFLibrary);
            
            // 使用存储/捕获到的jsPDF库创建PDF文档
            var doc = new jsPDFRef();
        
                        // 设置中文字体支持
            try {
                // 直接使用jsPDF内置字体，这些字体通常支持中文
                if (typeof doc.setFont === 'function') {
                    // 尝试设置支持中文的字体
                    try {
                        doc.setFont("helvetica");
                        console.log("使用helvetica字体（支持中文）");
                    } catch (e1) {
                        try {
                            doc.setFont("times");
                            console.log("使用times字体（支持中文）");
                        } catch (e2) {
                            try {
                                doc.setFont("courier");
                                console.log("使用courier字体（支持中文）");
                            } catch (e3) {
                                console.warn("所有字体设置失败，使用默认字体");
                            }
                        }
                    }
                }
                
                // 设置字体大小和样式
                doc.setFontSize(18);
                doc.setTextColor(0, 0, 0); // 黑色文字
                
            } catch (fontError) {
                console.warn("字体设置失败，使用默认字体:", fontError);
            }
            
            // 添加标题
            doc.setFontSize(18);
            doc.text("基于粤商文化的留学生商务汉语应用虚拟仿真实验-考核成绩", 105, 20, { align: "center" });
            
            // 添加学生信息
            doc.setFontSize(12);
            doc.text("班级：" + pdfData.studentClass, 20, 40);
            doc.text("姓名：" + pdfData.studentName, 20, 50);
            doc.text("学号：" + pdfData.studentId, 20, 60);
            doc.text("考核开始时间：" + pdfData.enterTime, 20, 70);
            doc.text("考核用时：" + pdfData.totalTime, 20, 80);
            doc.text("考核结束时间：" + pdfData.endTime, 20, 90);
            doc.text("总分：" + pdfData.totalScore, 20, 100);
            
            // 添加成绩表格标题
            doc.setFontSize(14);
            doc.text("详细成绩", 20, 120);
            
            // 创建成绩表格
            var startY = 130;
            var lineHeight = 8;
            
            // 表头
            doc.setFontSize(10);
            doc.text("序号", 20, startY);
            doc.text("步骤", 40, startY);
            doc.text("应得分数", 120, startY);
            doc.text("实际分数", 160, startY);
            
            startY += lineHeight;
            
            // 添加成绩数据
            for (var i = 0; i < pdfData.scores.length; i++) {
                var score = pdfData.scores[i];
                
                // 从Content获取步骤名称和应得分数
                var stepName = "";
                var expectedScore = "1"; // 默认值
                
                // 如果存在scoreItems数据，则从中获取
                if (pdfData.scoreItems && pdfData.scoreItems[i]) {
                    stepName = pdfData.scoreItems[i].step || "步骤" + (i + 1);
                    expectedScore = pdfData.scoreItems[i].expectedScore || "1";
                } else {
                    // 如果没有scoreItems数据，使用默认值
                    stepName = "步骤" + (i + 1);
                    expectedScore = "1";
                }
                
                doc.text((i + 1).toString(), 20, startY);
                doc.text(stepName, 40, startY);
                doc.text(expectedScore, 120, startY); // 从Content获取的应得分数
                doc.text(score.toString(), 160, startY);
                
                startY += lineHeight;
                
                // 如果内容太多，添加新页面
                if (startY > 250) {
                    doc.addPage();
                    startY = 20;
                }
            }
            
            // 保存PDF文件
            var fileName = pdfData.studentClass + "-" + pdfData.studentName + "-" + pdfData.studentId + ".pdf";
            doc.save(fileName);
            
            console.log("PDF生成成功：" + fileName);
            alert("PDF生成成功：" + fileName);
            
        } catch (error) {
            console.error('PDF生成过程中发生错误：', error);
            alert('PDF生成失败：' + error.message);
        }
    }
});
