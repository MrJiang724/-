function getdata(URL,div_name,func) {
    var result = new Object();
    $.ajax({
        method: "GET",
        url: URL,
        dataType: "json",
        success: function (data) {
            var chartData = [];
            var dataObj = data;
            var xdata = dataObj.xData;
            chartData[0] = { data: xdata };
            var dataKey = dataObj.data;
            var i = 0;
            for (var key in dataKey) {
                var name = key;
                i++;
                var data = dataObj.data[key];
                chartData[i] = { name: key, data: data };
            }
            func(div_name, dataObj.description, dataObj.title, chartData);
        }
    });
}


//生成产品报表
function createProductSummary(div_name,descripe,title, chartData)
{
    // 路径配置
    require.config({
        paths: {
            echarts: 'http://echarts.baidu.com/build/dist'
        }
    });

    // 使用
    require(
            [
                'echarts',
                'echarts/chart/bar', // 使用柱状图就加载bar模块，按需加载
                'echarts/chart/line'
            ],
            function (ec) {
                // 基于准备好的dom，初始化echarts图表
                var myChart = ec.init(document.getElementById(div_name));

                option = {
                    title: {
                        text: title,
                        subtext: descripe

                    },
                    tooltip: {
                        trigger: 'axis'
                    },
                    legend: {
                        y: 'bottom',
                        data: [chartData[1].name, chartData[2].name, chartData[3].name, chartData[4].name]
                    },
                    toolbox: {
                        show: true,
                        feature: {
                            mark: { show: true },
                            magicType: { show: true, type: ['line', 'bar'] },
                            restore: { show: true },
                            saveAsImage: { show: true }
                        }
                    },
                    calculable: true,
                    xAxis: [
                        {
                            type: 'category',
                            data: chartData[0].data
                        }
                    ],
                    yAxis: [
                        {
                            type: 'value'
                        }
                    ],
                    series: [
                        {
                            name: chartData[1].name,
                            type: 'bar',
                            data: chartData[1].data,
                            markPoint: {
                                data: [{ type: 'max', name: '最大值' }, { type: 'min', name: '最小值'}]
                            },
                            markLine: {
                                data: [{ type: 'average', name: '平均值'}]
                            }
                        },
                        {
                            name: chartData[2].name,
                            type: 'bar',
                            data: chartData[2].data,
                            markPoint: {
                                data: [{ type: 'max', name: '最大值' }, { type: 'min', name: '最小值'}]
                            },
                            markLine: {
                                data: [{ type: 'average', name: '平均值'}]
                            }
                        },
                        {
                            name: chartData[3].name,
                            type: 'bar',
                            data: chartData[3].data,
                            markPoint: {
                                data: [{ type: 'max', name: '最大值' }, { type: 'min', name: '最小值'}]
                            },
                            markLine: {
                                data: [{ type: 'average', name: '平均值'}]
                            }
                        },
                        {
                            name: chartData[4].name,
                            type: 'bar',
                            data: chartData[4].data,
                            markPoint: {
                                data: [{ type: 'max', name: '最大值' }, { type: 'min', name: '最小值'}]
                            },
                            markLine: {
                                data: [{ type: 'average', name: '平均值'}]
                            }
                        }
                    ]
                };

        // 为echarts对象加载数据 
        myChart.setOption(option);
    });

}

//生成Wifi报表
function createWifiChart(div_name, descripe, title, chartData) {
    // 路径配置
    require.config({
        paths: {
            echarts: 'http://echarts.baidu.com/build/dist'
        }
    });

    // 使用
    require(
            [
                'echarts',
                'echarts/chart/bar', // 使用柱状图就加载bar模块，按需加载
                'echarts/chart/line'
            ],
            function (ec) {
                // 基于准备好的dom，初始化echarts图表
                var myChart = ec.init(document.getElementById(div_name));

                option = {
                    title: {
                        text: title,
                        subtext: descripe
                    },
                    tooltip: {
                        trigger: 'axis'
                    },
                    legend: {
                        data: [chartData[1].name, chartData[2].name]
                    },
                    toolbox: {
                        show: true,
                        feature: {
                            mark: { show: true },
                            magicType: { show: true, type: ['line', 'bar'] },
                            restore: { show: true },
                            saveAsImage: { show: true }
                        }
                    },
                    calculable: true,
                    xAxis: [
                        {
                            type: 'category',
                            data: chartData[0].data
                        }
                    ],
                    yAxis: [
                        {
                            type: 'value'
                        }
                    ],
                    series: [
                        {
                            name: chartData[1].name,
                            type: 'bar',
                            data: chartData[1].data,
                            markPoint: {
                                data: [{ type: 'max', name: '最大值' }, { type: 'min', name: '最小值'}]
                            },
                            markLine: {
                                data: [{ type: 'average', name: '平均值'}]
                            }
                        },
                        {
                            name: chartData[2].name,
                            type: 'bar',
                            data: chartData[2].data,
                            markPoint: {
                                data: [{ type: 'max', name: '最大值' }, { type: 'min', name: '最小值'}]
                            },
                            markLine: {
                                data: [{ type: 'average', name: '平均值'}]
                            }
                        }
                    ]
                };

                // 为echarts对象加载数据 
                myChart.setOption(option);
            });
}




//生成广告机访问报表
function createAdvMachineChart(div_name, descripe, title, chartData) {
    // 路径配置
    require.config({
                paths: {
                    echarts: 'http://echarts.baidu.com/build/dist'
                }
            });

            // 使用
            require(
            [
                'echarts',
                'echarts/chart/bar', // 使用柱状图就加载bar模块，按需加载
                'echarts/chart/line'
            ],
            function (ec) {
                // 基于准备好的dom，初始化echarts图表
                var myChart = ec.init(document.getElementById(div_name));

                option = {
                    title: {
                        text: title,
                        subtext: descripe
                    },
                    tooltip: {
                        trigger: 'axis'
                    },
                    legend: {
                        data: [chartData[1].name, chartData[2].name, chartData[3].name, chartData[4].name, chartData[5].name, chartData[6].name]
                    },
                    toolbox: {
                        show: true,
                        feature: {
                            mark: { show: true },
                            magicType: { show: true, type: ['line', 'bar'] },
                            restore: { show: true },
                            saveAsImage: { show: true }
                        }
                    },
                    calculable: true,
                    xAxis: [
                        {
                            type: 'category',
                            data: chartData[0].data
                        }
                    ],
                    yAxis: [
                        {
                            type: 'value'
                        }
                    ],
                    series: [
                        {
                            name: chartData[1].name,
                            type: 'bar',
                            stack: '访问量',
                            data: chartData[1].data,
                            markPoint: {
                                data: [{ type: 'max', name: '最大值' }, { type: 'min', name: '最小值'}]
                            }
                        },
                        {
                            name: chartData[2].name,
                            type: 'bar',
                            stack: '访问量',
                            data: chartData[2].data,
                            markPoint: {
                                data: [{ type: 'max', name: '最大值' }, { type: 'min', name: '最小值'}]
                            }
                        },
                        {
                            name: chartData[3].name,
                            type: 'bar',
                            stack: '访问量',
                            data: chartData[3].data,
                            markPoint: {
                                data: [{ type: 'max', name: '最大值' }, { type: 'min', name: '最小值'}]
                            }
                        },
                        {
                            name: chartData[4].name,
                            type: 'bar',
                            stack: '访问量',
                            data: chartData[4].data,
                            markPoint: {
                                data: [{ type: 'max', name: '最大值' }, { type: 'min', name: '最小值'}]
                            }
                        },
                        {
                            name: chartData[5].name,
                            type: 'bar',
                            stack: '访问量',
                            data: chartData[5].data,
                            markPoint: {
                                data: [{ type: 'max', name: '最大值' }, { type: 'min', name: '最小值'}]
                            }
                        },
                        {
                            name: chartData[6].name,
                            type: 'bar',
                            data: chartData[6].data,
                            markPoint: {
                                data: [{ type: 'max', name: '最大值' }, { type: 'min', name: '最小值'}]
                            },
                            markLine: {
                                data: [{ type: 'average', name: '平均值'}]
                            }
                        }
                    ]
                };

                // 为echarts对象加载数据 
                myChart.setOption(option);
            });

        }



        //生成微信订阅情况报表
        function createWeixinTotalChart(div_name, descripe, title, chartData) {
            // 路径配置
            require.config({
                paths: {
                    echarts: 'http://echarts.baidu.com/build/dist'
                }
            });

            // 使用
            require(
            [
                'echarts',
                'echarts/chart/line'
            ],
            function (ec) {
                // 基于准备好的dom，初始化echarts图表
                var myChart = ec.init(document.getElementById(div_name));

                option = {
                    title: {
                        text: title,
                        subtext: descripe
                    },
                    tooltip: {
                        trigger: 'axis'
                    },
                    legend: {
                        data: [chartData[1].name]
                    },
                    toolbox: {
                        show: true,
                        feature: {
                            mark: { show: true },
                            restore: { show: true },
                            saveAsImage: { show: true }
                        }
                    },
                    calculable: true,
                    xAxis: [
                        {
                            type: 'category',
                            data: chartData[0].data
                        }
                    ],
                    yAxis: [
                        {
                            type: 'value',
                            scale: true,
                            boundaryGap: [0.01, 0.01]
                        }
                    ],
                    series: [
                        {
                            name: chartData[1].name,
                            type: 'line',
                            data: chartData[1].data
                        }
                    ]
                };

                // 为echarts对象加载数据 
                myChart.setOption(option);
            });

        }


        //生成微信活跃情况报表
        function createWeixinWeixinActivePeopleChart(div_name, descripe, title, chartData) {
            // 路径配置
            require.config({
                paths: {
                    echarts: 'http://echarts.baidu.com/build/dist'
                }
            });

            // 使用
            require(
            [
                'echarts',
                'echarts/chart/bar'
            ],
            function (ec) {
                // 基于准备好的dom，初始化echarts图表
                var myChart = ec.init(document.getElementById(div_name));

                option = {
                    title: {
                        text: title,
                        subtext: descripe
                    },
                    tooltip: {
                        trigger: 'axis'
                    },
                    legend: {
                        data: [chartData[1].name]
                    },
                    toolbox: {
                        show: true,
                        feature: {
                            mark: { show: true },
                            restore: { show: true },
                            saveAsImage: { show: true }
                        }
                    },
                    calculable: true,
                    xAxis: [
                        {
                            type: 'category',
                            data: chartData[0].data
                        }
                    ],
                    yAxis: [
                        {
                            type: 'value'
                        }
                    ],
                    series: [
                        {
                            name: chartData[1].name,
                            type: 'bar',
                            data: chartData[1].data
                        }
                    ]
                };

                // 为echarts对象加载数据 
                myChart.setOption(option);
            });

        }

        //生成过店人数情况报表
        function createInStornChart(div_name, descripe, title, chartData) {
            // 路径配置
            require.config({
                paths: {
                    echarts: 'http://echarts.baidu.com/build/dist'
                }
            });

            // 使用
            require(
            [
                'echarts',
                'echarts/chart/bar'
            ],
            function (ec) {
                // 基于准备好的dom，初始化echarts图表
                var myChart = ec.init(document.getElementById(div_name));

                option = {
                    title: {
                        text: title,
                        subtext: descripe
                    },
                    tooltip: {
                        trigger: 'axis'
                    },
                    legend: {
                        data: [chartData[1].name, chartData[2].name]
                    },
                    toolbox: {
                        show: true,
                        feature: {
                            mark: { show: true },
                            restore: { show: true },
                            saveAsImage: { show: true }
                        }
                    },
                    calculable: true,
                    xAxis: [
                        {
                            type: 'category',
                            data: chartData[0].data
                        }
                    ],
                    yAxis: [
                        {
                            type: 'value'
                        }
                    ],
                    series: [
                        {
                            name: chartData[1].name,
                            type: 'bar',
                            data: chartData[1].data,
                            markPoint: {
                                data: [{ type: 'max', name: '最大值' }, { type: 'min', name: '最小值'}]
                            },
                            markLine: {
                                data: [{ type: 'average', name: '平均值'}]
                            }
                        },
                        {
                            name: chartData[2].name,
                            type: 'bar',
                            data: chartData[2].data,
                            markPoint: {
                                data: [{ type: 'max', name: '最大值' }, { type: 'min', name: '最小值'}]
                            },
                            markLine: {
                                data: [{ type: 'average', name: '平均值'}]
                            }
                        }
                    ]
                };

                // 为echarts对象加载数据 
                myChart.setOption(option);
            });

        }